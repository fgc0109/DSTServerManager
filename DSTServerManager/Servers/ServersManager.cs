using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DSTServerManager.Servers
{
    /// <summary>
    /// 游戏服务器信息
    /// </summary>
    static class ServersManager
    {
        private static List<ServerConnect> m_ServerConnect = new List<ServerConnect>();
        private static List<ServerProcess> m_ServerProcess = new List<ServerProcess>();
        private static List<ServerScreens> m_ServerScreens = new List<ServerScreens>();
        private static List<TextBox> m_LogText = new List<TextBox>();

        private static string m_DefaultPath_CloudUser = string.Empty;
        private static string m_DefaultPath_CloudRoot = $"/var/run/screen/S-root";

        private static string m_TabItemXaml = null;
        private static Window m_MainWindows = null;
        private static TabControl m_TabCtrl = null;

        public static string TabItemXaml { set { m_TabItemXaml = value; } }
        public static Window MainWindows { set { m_MainWindows = value; } }
        public static TabControl TabCtrl { set { m_TabCtrl = value; } }

        internal static string CreatParameter(string confdir, string cluster, string servers)
        {
            StringBuilder cmdBuilder = new StringBuilder(256);
            cmdBuilder.Append($" -conf_dir {confdir}");
            cmdBuilder.Append($" -cluster {cluster}");
            cmdBuilder.Append($" -shard {servers}");

            return cmdBuilder.ToString();
        }

        internal static void CreatWindowTab(string tabName)
        {
            TabItem newProcessTab = System.Windows.Markup.XamlReader.Parse(m_TabItemXaml) as TabItem;
            newProcessTab.Header = tabName;

            m_MainWindows.Dispatcher.Invoke(new Action(() =>
            {
                m_TabCtrl.Items.Add(newProcessTab);
            }));

            foreach (var item in (newProcessTab.Content as Grid).Children) m_LogText.Add((TextBox)item);
        }

        internal static void CreatNewProcess(string serverExe, string parameter, string session)
        {
            if (!File.Exists(serverExe)) return;

            //ServerProcess process = new ServerProcess(false, session);
            //process.StartProcess(serverExe, parameter);
        }

        /// <summary>
        /// 获取已经存在的Process
        /// </summary>
        /// <returns></returns>
        internal static List<string> GetExistProcess()
        {
            List<string> process = new List<string>();
            return process;
        }

        /// <summary>
        /// 获取已经存在的Screens
        /// </summary>
        /// <param name="connect"></param>
        /// <returns></returns>
        internal static List<string> GetExistScreens(ServerConnect connect)
        {
            m_DefaultPath_CloudUser = $"/var/run/screen/S-{connect.UserName}";

            List<string> screens = new List<string>();

            IEnumerable<SftpFile> sftpFile = null;
            try { sftpFile = connect.GetSftpClient.ListDirectory(m_DefaultPath_CloudUser); }
            catch (SftpPathNotFoundException) { return screens; }
            catch (SftpPermissionDeniedException) { throw; }
            catch (Exception) { throw; }

            foreach (var item in sftpFile)
                if (item.Name != "." && item.Name != "..") screens.Add(item.Name);
            return screens;
        }
    }
}
