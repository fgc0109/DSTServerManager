using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;

namespace DSTServerManager.Saves
{
    /// <summary>
    /// 服务器集群信息
    /// </summary>
    class ClusterInfo
    {
        private string clusterFolder = string.Empty;
        private ClusterIni clusterSetting = new ClusterIni();
        private List<ServerInfo> clusterServers = new List<ServerInfo>();        

        public string ClusterFolder
        {
            get { return clusterFolder; }
            set { clusterFolder = value; }
        }

        public ClusterIni ClusterSetting
        {
            get { return clusterSetting; }
            set { clusterSetting = value; }
        }

        public List<ServerInfo> ClusterServers
        {
            get { return clusterServers; }
            set { clusterServers = value; }
        }
    }
}
