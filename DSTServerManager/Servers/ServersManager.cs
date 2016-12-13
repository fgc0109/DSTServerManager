using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Renci.SshNet.Common;

namespace DSTServerManager.Servers
{
    /// <summary>
    /// 游戏服务器信息
    /// </summary>
    class ServersManager
    {
        ServerConnect m_ServerConnect = null;
        private static string m_DefaultPath_CloudUser = string.Empty;
        private static string m_DefaultPath_CloudRoot = $"/var/run/screen/S-root";


        public string CreatParameter()
        {
            return string.Empty;
        }

        /// <summary>
        /// 获取已经存在的Process
        /// </summary>
        /// <returns></returns>
        public List<string> GetExistProcess()
        {
            List<string> process = new List<string>();
            return process;
        }

        /// <summary>
        /// 获取已经存在的Screens
        /// </summary>
        /// <param name="connect"></param>
        /// <returns></returns>
        public List<string> GetExistScreens(ServerConnect connect)
        {
            m_DefaultPath_CloudUser = $"/var/run/screen/S-{connect.UserName}";

            List<string> screens = new List<string>();
            m_ServerConnect = connect;

            IEnumerable<SftpFile> sftpFile = null;
            try { sftpFile = m_ServerConnect.GetSftpClient.ListDirectory(m_DefaultPath_CloudUser); }
            catch (SftpPathNotFoundException) { return screens; }
            catch (SftpPermissionDeniedException) { throw; }
            catch (Exception) { throw; }

            foreach (var item in sftpFile)
                if (item.Name != "." && item.Name != "..") screens.Add(item.Name);
            return screens;
        }
    }
}
