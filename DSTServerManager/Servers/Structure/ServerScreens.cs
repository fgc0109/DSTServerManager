using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Text.RegularExpressions;

namespace DSTServerManager.Servers
{
    /// <summary>
    /// 用来处理Linux会话
    /// </summary>
    class ServerScreens
    {
        private SftpClient m_SftpClient;
        private SshClient m_SshClient;
        private ScpClient m_ScpClient;

        private string m_DefaultPathRoot = @"/root";
        private string m_DefaultPathUser = @"/home/{0}";

        private bool m_AllConnected = false;
        private string m_LogInfos = string.Empty;
        private string m_ScreenName = string.Empty;
        private ShellStream m_ShellStream = null;

        private TabControl m_TabControl = null;
        private TabItem m_ScreensTab = null;
        private TextBox m_ScreensLog = null;

        public ServerScreens(string ip, string userName, string password)
        {
            m_SftpClient = new SftpClient(ip, 22, userName, password);
            m_SshClient = new SshClient(ip, userName, password);
            m_ScpClient = new ScpClient(ip, userName, password);
        }

        public SftpClient GetSftpClient { get { return m_SftpClient; } }
        public SshClient GetSshClient { get { return m_SshClient; } }
        public ScpClient GetScpClient { get { return m_ScpClient; } }
        public TabItem ServerTab { get { return m_ScreensTab; } }

        public void CreatTabWindow(TabControl tabControl, TabItem tabItem)
        {
            m_ScreensTab = tabItem;
            m_TabControl = tabControl;
            foreach (var item in (tabItem.Content as Grid).Children) m_ScreensLog = (TextBox)item;

            if (m_LogInfos != string.Empty) tabControl.Dispatcher.Invoke(new Action(WriteTextLogs));
        }

        /// <summary>
        /// 开启Screens
        /// </summary>
        public void StartScreens()
        {
            try
            {
                if (m_SftpClient.IsConnected == false) m_SftpClient.Connect();
                if (m_SshClient.IsConnected == false) m_SshClient.Connect();
                if (m_ScpClient.IsConnected == false) m_ScpClient.Connect();

                m_AllConnected = true;
            }
            catch { throw; }

            //m_ShellStream = m_SshClient.CreateShellStream("anything", 80, 24, 800, 600, 4096);
            //m_ShellStream = m_SshClient.CreateShellStream("xterm", 80, 24, 800, 600, 4096);
            m_ShellStream = m_SshClient.CreateShellStream("putty-vt100", 80, 24, 800, 600, 4096);
            byte[] buffer = new byte[4096];

            m_ShellStream.DataReceived += new EventHandler<ShellDataEventArgs>(Screens_OutputDataReceived);
            m_ShellStream.ReadAsync(buffer, 0, buffer.Length);
        }
        public void SendCommand(string command)
        {
            //byte[] buffer =Encoding.UTF8.GetBytes(command);
            //m_ShellStream.WriteAsync(buffer, 0, buffer.Length);

            m_ShellStream.WriteLine(command);
        }

        Regex color = new Regex("\\[[^ ]*?m", RegexOptions.Compiled);
        Regex lines = new Regex("%[ ]*?\\r", RegexOptions.Compiled);
        Regex test1 = new Regex("\\r", RegexOptions.Compiled);

        private void Screens_OutputDataReceived(object sender, ShellDataEventArgs received)
        {
            m_LogInfos += Encoding.UTF8.GetString(received.Data);

            m_LogInfos = color.Replace(m_LogInfos, "");
            m_LogInfos = lines.Replace(m_LogInfos, "");
            m_LogInfos = test1.Replace(m_LogInfos, "");

            if (m_ScreensLog == null) return;
            m_TabControl.Dispatcher.Invoke(new Action(WriteTextLogs));
            m_LogInfos = string.Empty;
        }

        /// <summary>
        /// 向TextBox控件写入Log信息
        /// </summary>
        /// <param name="logInfo"></param>
        private void WriteTextLogs()
        {
            m_ScreensLog.Text += m_LogInfos + "\r\n";
            m_LogInfos = string.Empty;
            m_ScreensLog.CaretIndex = m_ScreensLog.Text.Length;
            m_ScreensLog.ScrollToEnd();
        }
    }
}
