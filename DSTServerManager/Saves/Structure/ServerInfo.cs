using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTServerManager.Saves
{
    /// <summary>
    /// 服务器配置文件
    /// </summary>
    class ServerInfo
    {
        private string folder = string.Empty;
        private string session = string.Empty;
        private ServerIni setting = new ServerIni();
        private ServerLevel level = new ServerLevel();

        public string Folder
        {
            get { return folder; }
            set { folder = value; }
        }

        public string Session
        {
            get { return session; }
            set { session = value; }
        }

        public ServerIni Setting
        {
            get { return setting; }
            set { setting = value; }
        }

        public ServerLevel Level
        {
            get { return level; }
            set { level = value; }
        }
    }
}
