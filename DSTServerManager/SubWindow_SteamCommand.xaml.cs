using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DSTServerManager
{
    /// <summary>
    /// SubWindow_SteamCommandLine.xaml 的交互逻辑
    /// </summary>
    public partial class SubWindow_SteamCommand : Window
    {
        public delegate void SteamCommandHandler(object sender, SteamCommandEventArgs e);
        public event SteamCommandHandler SteamCommandEvent;

        private Process m_ServerProcess = null;

        public SubWindow_SteamCommand()
        {
            InitializeComponent();
        }



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




    }

    /// <summary>
    /// 容纳参数传递事件的附加信息
    /// </summary>
    public class SteamCommandEventArgs : EventArgs
    {
        private readonly bool m_NewRow;

        public SteamCommandEventArgs(bool newRow)
        {
            m_NewRow = newRow;
        }
        public bool IsNewRow { get { return m_NewRow; } }
    }
}
