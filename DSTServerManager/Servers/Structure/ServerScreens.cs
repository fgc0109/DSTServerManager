using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace DSTServerManager.Servers
{
    /// <summary>
    /// 用来处理Linux会话
    /// </summary>
    class ServerScreens
    {
        private SshClient m_SshClient;
        private SftpClient m_SftpClient;
        private ScpClient m_ScpClient;

        public ServerScreens(string ip, string port, string user, string password)
        {
            m_SftpClient = new SftpClient(ip, Int32.Parse(port), user, password);
            m_SshClient = new SshClient(ip, user, password);
            m_ScpClient = new ScpClient(ip, user, password);
        }

        public bool Connected { get { return m_SftpClient.IsConnected; } }

        public bool Connect()
        {
            try
            {
                if (!Connected)
                {
                    m_SftpClient.Connect();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("连接SFTP失败，原因：{0}", ex.Message));
            }

            // m_SftpClient.
            //m_SshClient.
        }
    }
}
