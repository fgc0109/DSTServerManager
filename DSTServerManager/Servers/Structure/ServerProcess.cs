using System;
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
    /// 本地服务器Process
    /// </summary>
    class ServerProcess
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);

        private Process m_ServerProcess = null;
        private string m_ServerSession = string.Empty;
        private bool m_ProcessActive = false;

        //用于重定向输出后的界面创建和显示
        private bool m_IsShellWin = true;

        private TabControl m_TabControl = null;
        private TabItem m_ProcessTab = null;
        private TextBox m_ProcessLog = null;
        private StreamWriter m_StreamWriter = null;

        /// <summary>
        /// 创建一个新的Process实例
        /// </summary>
        /// <param name="tabControl"></param>
        /// <param name="tabItem">包含TextBox子控件的TabItem控件</param>
        /// <param name="isShell"></param>
        /// <param name="session"></param>
        public ServerProcess(bool isShell, string session)
        {
            m_IsShellWin = isShell;
            m_ServerSession = session;
            m_ServerProcess = new Process();
        }

        public TabItem ServerTab { get { return m_ProcessTab; } }
        public bool IsProcessActive { get { return m_ProcessActive; } }
        public string ServerSession { get { return m_ServerSession; } }

        /// <summary>
        /// 开启Process
        /// </summary>
        /// <param name="fullpath">程序全路径</param>
        /// <param name="command">程序启动参数</param>
        public void StartProcess(string fullpath, string command)
        {
            //获取工作目录
            string directory = string.Empty;
            string[] folder = fullpath.Split('\\');
            for (int i = 0; i < folder.Length - 1; i++)
                directory += folder[i] + "\\";

            //设置启动参数
            m_ServerProcess.StartInfo.WorkingDirectory = directory;
            m_ServerProcess.StartInfo.FileName = fullpath;
            m_ServerProcess.StartInfo.Arguments = command;
            if (!m_IsShellWin)
            {
                m_ServerProcess.StartInfo.UseShellExecute = false;
                m_ServerProcess.StartInfo.RedirectStandardInput = true;
                m_ServerProcess.StartInfo.RedirectStandardOutput = true;
                m_ServerProcess.StartInfo.RedirectStandardError = true;
                m_ServerProcess.StartInfo.CreateNoWindow = true;

                m_ServerProcess.OutputDataReceived += new DataReceivedEventHandler(Process_OutputDataReceived);
            }
            m_ServerProcess.EnableRaisingEvents = true;
            m_ServerProcess.Exited += new EventHandler(Process_Exited);

            m_ServerProcess.Start();
            if (!m_IsShellWin)
            {
                m_ServerProcess.BeginOutputReadLine();

                m_StreamWriter = m_ServerProcess.StandardInput;
                m_StreamWriter.AutoFlush = true;
            }
            m_ProcessActive = true;
        }

        public void CreatTabWindow(TabControl tabControl, TabItem tabItem)
        {
            m_ProcessTab = tabItem;
            m_TabControl = tabControl;
            foreach (var item in (tabItem.Content as Grid).Children) m_ProcessLog = (TextBox)item;
        }

        /// <summary>
        /// 向Process发送控制台命令
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(string command)
        {
            if (!m_IsShellWin)
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

        private void Process_Exited(object sender, EventArgs e)
        {
            m_ProcessActive = false;
            m_ServerProcess = null;

            m_TabControl.Dispatcher.Invoke(new Action(() => { m_TabControl.Items.Remove(m_ProcessTab); }));
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs received)
        {
            m_TabControl.Dispatcher.Invoke(new Action(() =>
            {
                m_ProcessLog.Text += received.Data + "\r\n";
                m_ProcessLog.CaretIndex = m_ProcessLog.Text.Length;
                m_ProcessLog.ScrollToEnd();
            }));
        }
    }
}
