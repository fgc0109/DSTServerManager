using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace DSTServerManager.Servers
{
    /// <summary>
    /// 远程服务器连接
    /// </summary>
    class ServerConnect
    {
        private SftpClient m_SftpClient;
        private SshClient m_SshClient;
        private ScpClient m_ScpClient;

        private bool m_AllConnected = false;
        private string m_LogInfos = string.Empty;
        private ShellStream m_ShellStream = null;
        
        private TabControl m_TabControl = null;
        private TabItem m_ConnectTab = null;
        private TextBox m_ConnectLog = null;

        private string m_UserName = string.Empty;
        private string m_Password = string.Empty;
        private string m_IPAddres = string.Empty;

        /// <summary>
        /// 初始化连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public ServerConnect(string ip, string userName, string password)
        {
            m_SftpClient = new SftpClient(ip, 22, userName, password);
            m_SshClient = new SshClient(ip, userName, password);
            m_ScpClient = new ScpClient(ip, userName, password);

            m_UserName = userName;
            m_Password = password;
            m_IPAddres = ip;
        }

        public SftpClient GetSftpClient { get { return m_SftpClient; } }
        public SshClient GetSshClient { get { return m_SshClient; } }
        public ScpClient GetScpClient { get { return m_ScpClient; } }
        public TabItem ServerTab { get { return m_ConnectTab; } }

        public bool AllConnected { get { return m_AllConnected; } }

        public string UserName { get { return m_UserName; } }
        public string Password { get { return m_Password; } }
        public string IPAddres { get { return m_IPAddres; } }

        /// <summary>
        /// 开启Connect
        /// </summary>
        /// <returns></returns>
        public void StartConnect()
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

            m_ShellStream.DataReceived += new EventHandler<ShellDataEventArgs>(connect_OutputDataReceived);
            m_ShellStream.ReadAsync(buffer, 0, buffer.Length);
        }

        public void CreatTabWindow(TabControl tabControl, TabItem tabItem)
        {
            m_ConnectTab = tabItem;
            m_TabControl = tabControl;
            foreach (var item in (tabItem.Content as Grid).Children) m_ConnectLog = (TextBox)item;

            if (m_LogInfos != string.Empty) tabControl.Dispatcher.Invoke(new Action(WriteTextLogs));
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

        private void connect_OutputDataReceived(object sender, ShellDataEventArgs received)
        {
            m_LogInfos += Encoding.UTF8.GetString(received.Data);

            m_LogInfos = color.Replace(m_LogInfos, "");
            m_LogInfos = lines.Replace(m_LogInfos, "");
            m_LogInfos = test1.Replace(m_LogInfos, "");

            if (m_ConnectLog == null) return;
            m_TabControl.Dispatcher.Invoke(new Action(WriteTextLogs));
            m_LogInfos = string.Empty;
        }

        /// <summary>
        /// 向TextBox控件写入Log信息
        /// </summary>
        /// <param name="logInfo"></param>
        private void WriteTextLogs()
        {
            m_ConnectLog.Text += m_LogInfos;
            m_ConnectLog.CaretIndex = m_ConnectLog.Text.Length;
            m_ConnectLog.ScrollToEnd();
        }
    }


    /// <summary>
    /// 容纳参数传递事件的附加信息
    /// </summary>
    public class SteamCommandEventArgs : EventArgs
    {
        private readonly string m_NewServerPath;

        public SteamCommandEventArgs(string NewServerPath)
        {
            m_NewServerPath = NewServerPath;
        }
        public string NewServerPath { get { return m_NewServerPath; } }
    }
}
