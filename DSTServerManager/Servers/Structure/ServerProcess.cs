﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DSTServerManager.DataHelper;
using DSTServerManager.Saves;
using DSTServerManager.Servers;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DSTServerManager.Servers
{
    /// <summary>
    /// 
    /// </summary>
    class ServerProcess
    {
        Process m_ServerProcess = null;
        string m_ServerSession = string.Empty;
        bool m_ProcessActive = false;

        //用于重定向输出后的界面创建和显示
        bool m_UserInterface = true;
        Window m_MainWindow = null;
        TabControl m_TabControl = null;
        TabItem m_ServerTab = null;
        TextBox m_ServerLog = null;
        StreamWriter m_StreamWriter = null;

        /// <summary>
        /// 创建一个新的Process实例
        /// </summary>
        /// <param name="window">重定向输出的窗口进程</param>
        /// <param name="tabControl"></param>
        /// <param name="tabItem">包含TextBox子控件的TabItem控件</param>
        /// <param name="ui"></param>
        /// <param name="session"></param>
        public ServerProcess(Window window, TabControl tabControl, TabItem tabItem, bool ui, string session)
        {
            //获取重定向后的窗口和输出控件
            m_UserInterface = ui;
            m_ServerTab = tabItem;
            m_MainWindow = window;
            m_TabControl = tabControl;
            foreach (var item in (tabItem.Content as Grid).Children)
                m_ServerLog = (TextBox)item;

            m_ServerProcess = new Process();
            m_ServerSession = session;
        }

        public TabItem ServerTab
        { get { return m_ServerTab; } }

        public bool IsProcessActive
        { get { return m_ProcessActive; } }

        public string ServerSession
        { get { return m_ServerSession; } }

        /// <summary>
        /// 不使用windows外壳程序并且不显示窗口,所有信息输出到重定向的TextBox
        /// </summary>
        /// <param name="fullpath">程序全路径</param>
        /// <param name="argument">程序启动参数</param>
        public void StartProcess(string fullpath, params string[] argument)
        {
            //获取工作目录
            string directory = string.Empty;
            string[] folder = fullpath.Split('\\');
            for (int i = 0; i < folder.Length - 1; i++)
                directory += folder[i] + "\\";

            //获取完整命令
            string command = string.Empty;
            foreach (var argue in argument)
                command += argue;

            //设置启动参数
            m_ServerProcess.StartInfo.WorkingDirectory = directory;
            m_ServerProcess.StartInfo.FileName = fullpath;
            m_ServerProcess.StartInfo.Arguments = command;
            if (!m_UserInterface)
            {
                m_ServerProcess.StartInfo.UseShellExecute = false;
                m_ServerProcess.StartInfo.RedirectStandardInput = true;
                m_ServerProcess.StartInfo.RedirectStandardOutput = true;
                m_ServerProcess.StartInfo.RedirectStandardError = true;
                m_ServerProcess.StartInfo.CreateNoWindow = true;

                m_ServerProcess.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
            }
            m_ServerProcess.EnableRaisingEvents = true;
            m_ServerProcess.Exited += new EventHandler(process_Exited);

            m_ServerProcess.Start();
            if (!m_UserInterface)
            {
                m_ServerProcess.BeginOutputReadLine();

                m_StreamWriter = m_ServerProcess.StandardInput;
                m_StreamWriter.AutoFlush = true;
            }
            m_ProcessActive = true;
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);
        /// <summary>
        /// 向Process发送控制台命令
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(string command)
        {
            if (!m_UserInterface)
            {
                m_StreamWriter = m_ServerProcess.StandardInput;

                m_StreamWriter.WriteLine(command);
                m_StreamWriter.Flush();

            }
            else
            {
                IntPtr mainWindowHandle = m_ServerProcess.MainWindowHandle;
                SetForegroundWindow(mainWindowHandle);
                SetActiveWindow(mainWindowHandle);
                System.Windows.Forms.SendKeys.SendWait(command);
            }
        }


        private void process_Exited(object sender, EventArgs e)
        {
            m_ServerProcess = null;
            if (m_ServerTab != null)
                m_MainWindow.Dispatcher.Invoke(new Action(() => { m_TabControl.Items.Remove(m_ServerTab); }));

            m_ProcessActive = false;
        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs received)
        {
            m_MainWindow.Dispatcher.Invoke(new Action(() =>
            {
                m_ServerLog.Text += received.Data + "\r\n";
                m_ServerLog.CaretIndex = m_ServerLog.Text.Length;
                m_ServerLog.ScrollToEnd();
            }));
        }
    }
}
