using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private static string m_DefaultPath_CloudUser = string.Empty;
        private static string m_DefaultPath_CloudRoot = $"/var/run/screen/S-root";

        private static string m_TabItemXaml = null;
        private static TabControl m_TabCtrl = null;

        public static string TabItemXaml { set { m_TabItemXaml = value; } }
        public static TabControl TabCtrl { set { m_TabCtrl = value; } }

        /// <summary>
        /// 创建服务器启动参数
        /// </summary>
        /// <param name="confdir">服务器存档文件路径</param>
        /// <param name="cluster">服务器集群存档文件夹名</param>
        /// <param name="servers">服务器分片存档文件夹名</param>
        /// <returns></returns>
        internal static string CreatParameter(string confdir, string cluster, string servers)
        {
            StringBuilder cmdBuilder = new StringBuilder(256);
            cmdBuilder.Append($" -conf_dir {confdir}");
            cmdBuilder.Append($" -cluster {cluster}");
            cmdBuilder.Append($" -shard {servers}");

            return cmdBuilder.ToString();
        }

        /// <summary>
        /// 发送控制命令
        /// </summary>
        /// <param name="command"></param>
        internal static void SendCommand(string command)
        {
            foreach (var server in m_ServerProcess)
            {
                if (!server.ServerTab.Equals(m_TabCtrl.SelectedItem)) continue;

                server.SendCommand(command);
            }
        }

        /// <summary>
        /// 状态列表检查
        /// </summary>
        internal static void RefreshList()
        {
            for (int i = 0; i < m_ServerProcess.Count; i++)
            {
                if (m_ServerProcess[i].IsProcessActive == true) continue;

                m_ServerProcess[i] = null;
                m_ServerProcess.Remove(m_ServerProcess[i]);
            }
        }

        #region [本地Process]----------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// 创建本地Process
        /// </summary>
        /// <param name="serverExe"></param>
        /// <param name="parameter"></param>
        /// <param name="session"></param>
        internal static void CreatNewProcess(string serverExe, string parameter, string session)
        {
            if (!File.Exists(serverExe)) return;

            RefreshList();
            TabItem newProcessTab = System.Windows.Markup.XamlReader.Parse(m_TabItemXaml) as TabItem;
            m_TabCtrl.Dispatcher.Invoke(new Action(() => { m_TabCtrl.Items.Add(newProcessTab); }));

            ServerProcess process = new ServerProcess(m_TabCtrl, newProcessTab, false, session);

            //在后台线程开始执行
            //BackgroundWorker processWorker = new BackgroundWorker();
            //processWorker.DoWork +=ProcessWorker_DoWork;
            //processWorker.RunWorkerCompleted += ProcessWorker_RunWorkerCompleted;
            //processWorker.RunWorkerAsync(indexConn);

            process.StartProcess(serverExe, parameter);

            m_ServerProcess.Add(process);
        }

        private static void ProcessWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void ProcessWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion ----------------------------------------------------------------------------------------------------

        #region [远程Connect]----------------------------------------------------------------------------------------------------

        #endregion ----------------------------------------------------------------------------------------------------

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
