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
        string defaultPath = string.Empty;

        public void GetExistProcess()
        {

        }

        public List<string> GetExistScreens(ServerConnect connect)
        {
            defaultPath = $"/var/run/screen/S-{connect.UserName}";

            List<string> screen = new List<string>();
            m_ServerConnect = connect;

            IEnumerable<SftpFile> sftpFile = null;
            try { sftpFile = m_ServerConnect.GetSftpClient.ListDirectory(defaultPath); }
            catch (SftpPathNotFoundException) { return screen; }
            catch (SftpPermissionDeniedException) { throw; }
            catch (Exception) { throw; }

            foreach (var item in sftpFile)
                if (item.Name != "." && item.Name != "..") screen.Add(item.Name);
            return screen;
        }
    }
}
