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
        private static Regex color = new Regex("\\[[^ ]*?m", RegexOptions.Compiled);
        private static Regex lines = new Regex("%[ ]*?", RegexOptions.Compiled);
        private static Regex enter = new Regex("[ ]*\\r", RegexOptions.Compiled);
        private static Regex othr1 = new Regex("\\]\\d;[\\s\\S]+", RegexOptions.Compiled);
        private static Regex othr2 = new Regex("[ ]*\\[K[\\s\\S]?", RegexOptions.Compiled);

        private bool isLogOutput = true;
        private ShellStream m_ShellStream = null;
        private StringBuilder screensLogBuffer = new StringBuilder(4096);

        private SftpClient m_SftpClient;
        private SshClient m_SshClient;

        private string m_DefaultPathRoot = @"/root";
        private string m_DefaultPathUser = @"/home/{0}";

        private string m_ScreenName = string.Empty;

        private TabControl m_TabControl = null;
        private TabItem m_ScreensTab = null;
        private TextBox m_ScreensLog = null;

        public ServerScreens(string ip, string userName, string password)
        {
            m_SftpClient = new SftpClient(ip, 22, userName, password);
            m_SshClient = new SshClient(ip, userName, password);
        }

        public SftpClient GetSftpClient { get { return m_SftpClient; } }
        public SshClient GetSshClient { get { return m_SshClient; } }
        public TabItem ServerTab { get { return m_ScreensTab; } }
        public bool IsLogOutput { set { isLogOutput = value; } }

        public void CreatTabWindow(TabControl tabControl, TabItem tabItem)
        {
            m_ScreensTab = tabItem;
            m_TabControl = tabControl;
            foreach (var item in (tabItem.Content as Grid).Children) m_ScreensLog = (TextBox)item;
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
            }
            catch { throw; }

            m_ShellStream = m_SshClient.CreateShellStream("putty-vt100", 80, 24, 800, 600, 4096);
            byte[] buffer = new byte[4096];

            m_ShellStream.DataReceived += new EventHandler<ShellDataEventArgs>(Screens_OutputDataReceived);
            m_ShellStream.ReadAsync(buffer, 0, buffer.Length);
        }

        public void SendCommand(string command)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(command + "\r");
            m_ShellStream.WriteAsync(buffer, 0, buffer.Length);

            m_ShellStream.FlushAsync();
        }

        private void Screens_OutputDataReceived(object sender, ShellDataEventArgs received)
        {
            string data = Encoding.UTF8.GetString(received.Data);
            data = color.Replace(data, "");
            data = lines.Replace(data, "");
            data = enter.Replace(data, "");
            data = othr1.Replace(data, "");
            data = othr2.Replace(data, "");
            screensLogBuffer.Append(data);

            if (isLogOutput == false) return;
            DisplayData();
        }

        public void DisplayData()
        {
            string screensLog = screensLogBuffer.ToString();
            screensLogBuffer.Clear();

            m_TabControl.Dispatcher.Invoke(new Action(() =>
            {
                m_ScreensLog.Text += screensLog;
                screensLog = string.Empty;
                m_ScreensLog.CaretIndex = m_ScreensLog.Text.Length;
                m_ScreensLog.ScrollToEnd();
            }));
        }
    }
}
