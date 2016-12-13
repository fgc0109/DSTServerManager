using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using Renci.SshNet;

namespace DSTServerManager.Servers
{
    /// <summary>
    /// 用来处理Linux会话
    /// </summary>
    class ServerScreens
    {
        ServerConnect m_ServerConnect = null;
        private string m_DefaultPathRoot = @"/root";
        private string m_DefaultPathUser = @"/home/{0}";

        private string m_LogInfos = string.Empty;
        private string m_ScreenName = string.Empty;

        private Window m_MainWindow = null;
        private TabControl m_TabControl = null;
        private TabItem m_ServerTab = null;
        private TextBox m_ServerLog = null;

        public ServerScreens(ServerConnect connect)
        {
            m_ServerConnect = new ServerConnect(connect.IPAddres, connect.UserName, connect.Password);
            if (!m_ServerConnect.AllConnected) return;

            m_DefaultPathUser = string.Format(m_DefaultPathUser, m_ServerConnect.UserName);
        }

        public void CreatTabWindow(Window window, TabControl tabControl, TabItem tabItem)
        {
            m_ServerTab = tabItem;
            m_MainWindow = window;
            m_TabControl = tabControl;
            foreach (var item in (tabItem.Content as Grid).Children) m_ServerLog = (TextBox)item;

            if (m_LogInfos != string.Empty) m_MainWindow.Dispatcher.Invoke(new Action(WriteTextLogs));
        }

        public void AttachScreen(string screenName)
        {
            //复制一个链接

            //发送命令恢复窗口
        }

        /// <summary>
        /// 向TextBox控件写入Log信息
        /// </summary>
        /// <param name="logInfo"></param>
        private void WriteTextLogs()
        {
            m_ServerLog.Text += m_LogInfos + "\r\n";
            m_LogInfos = string.Empty;
            m_ServerLog.CaretIndex = m_ServerLog.Text.Length;
            m_ServerLog.ScrollToEnd();
        }
    }
}
