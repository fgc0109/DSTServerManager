using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ServerConnect(string ip, int port, string user, string password)
        {
            m_SftpClient = new SftpClient(ip, port, user, password);
            m_SshClient = new SshClient(ip, user, password);
            m_ScpClient = new ScpClient(ip, user, password);
        }

        public SftpClient GetSftpClient { get { return m_SftpClient; } }
        public SshClient GetSshClient { get { return m_SshClient; } }
        public ScpClient GetScpClient { get { return m_ScpClient; } }
    }
}
