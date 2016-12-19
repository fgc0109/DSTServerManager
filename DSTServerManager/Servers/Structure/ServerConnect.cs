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
        Regex color = new Regex("\\[[^ ]*?m", RegexOptions.Compiled);
        Regex lines = new Regex("%[ ]*?\\r", RegexOptions.Compiled);
        Regex enter = new Regex("[ ]*\\r", RegexOptions.Compiled);

        private bool isLogOutput = true;
        private ShellStream m_ShellStream = null;
        private StringBuilder connectLogBuffer = new StringBuilder(4096);

        private SftpClient m_SftpClient;
        private SshClient m_SshClient;
        private ScpClient m_ScpClient;

        private bool m_AllConnected = false;

        private TabControl m_TabControl = null;
        private TabItem m_ConnectTab = null;
        private TextBox m_ConnectLog = null;

        private string m_UserName = string.Empty;
        private string m_Password = string.Empty;
        private string m_Location = string.Empty;

        /// <summary>
        /// 初始化连接
        /// </summary>
        /// <param name="location"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public ServerConnect(string location, string username, string password)
        {
            m_SftpClient = new SftpClient(location, 22, username, password);
            m_SshClient = new SshClient(location, username, password);
            m_ScpClient = new ScpClient(location, username, password);

            m_UserName = username;
            m_Password = password;
            m_Location = location;
        }

        public SftpClient GetSftpClient { get { return m_SftpClient; } }
        public SshClient GetSshClient { get { return m_SshClient; } }
        public ScpClient GetScpClient { get { return m_ScpClient; } }
        public TabItem ServerTab { get { return m_ConnectTab; } }
        public bool IsLogOutput { set { isLogOutput = value; } }

        public bool AllConnected { get { return m_AllConnected; } }

        public string Username { get { return m_UserName; } }
        public string Password { get { return m_Password; } }
        public string Location { get { return m_Location; } }

        /// <summary>
        /// 开启Connect
        /// </summary>
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

            m_ShellStream = m_SshClient.CreateShellStream("anything", 80, 24, 800, 600, 4096);
            byte[] buffer = new byte[4096];

            m_ShellStream.DataReceived += new EventHandler<ShellDataEventArgs>(Connect_OutputDataReceived);
            m_ShellStream.ReadAsync(buffer, 0, buffer.Length);
        }

        public void CreatTabWindow(TabControl tabControl, TabItem tabItem)
        {
            m_ConnectTab = tabItem;
            m_TabControl = tabControl;
            foreach (var item in (tabItem.Content as Grid).Children) m_ConnectLog = (TextBox)item;
        }

        public void SendCommand(string command)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(command + "\r");
            m_ShellStream.WriteAsync(buffer, 0, buffer.Length);

            m_ShellStream.FlushAsync();
        }

        private void Connect_OutputDataReceived(object sender, ShellDataEventArgs received)
        {
            string data = Encoding.UTF8.GetString(received.Data);
            data = color.Replace(data, "");
            data = lines.Replace(data, "");
            data = enter.Replace(data, "");
            connectLogBuffer.Append(data);

            if (isLogOutput == false) return;
            DisplayData();
        }

        public void DisplayData()
        {
            string connectLog = connectLogBuffer.ToString();
            connectLogBuffer.Clear();

            m_TabControl.Dispatcher.Invoke(new Action(() =>
            {
                m_ConnectLog.Text += connectLog;
                connectLog = string.Empty;
                m_ConnectLog.CaretIndex = m_ConnectLog.Text.Length;
                m_ConnectLog.ScrollToEnd();
            }));
        }
    }
}
