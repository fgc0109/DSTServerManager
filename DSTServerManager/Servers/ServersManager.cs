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
using DSTServerManager.Saves;
using System.Windows;
using System.Windows.Controls;

namespace DSTServerManager.Servers
{
    /// <summary>
    /// 游戏服务器信息
    /// </summary>
    static class ServersManager
    {
        private static string m_DefaultPath_CloudUser = string.Empty;
        private static string m_DefaultPath_CloudRoot = $"/var/run/screen/S-root";

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
