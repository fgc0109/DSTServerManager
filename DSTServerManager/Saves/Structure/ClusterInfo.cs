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
    class ClusterInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string folder = string.Empty;
        private ClusterIni setting = new ClusterIni();
        private List<ServerInfo> servers = new List<ServerInfo>();

        DataTable clusterServerTable = new DataTable();

        public ClusterInfo(int columns)
        {
            for (int i = 0; i < columns; i++)
                clusterServerTable.Columns.Add(i.ToString());
        }

        public DataTable ClusterServerTable
        {
            get
            {
                FieldsToGrid();
                return clusterServerTable;
            }
            set
            {
                clusterServerTable = value;
                GridToFields();
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ClusterServerTable"));
            }
        }

        public string Folder
        {
            get { return folder; }
            set { folder = value; }
        }

        public ClusterIni Setting
        {
            get { return setting; }
            set { setting = value; }
        }

        public List<ServerInfo> Servers
        {
            get { return servers; }
            set { servers = value; }
        }

        /// <summary>
        /// 将属性转化为DataGrid
        /// </summary>
        /// <param name="columns"></param>
        public void FieldsToGrid()
        {

            foreach (var serverInfo in servers)
            {
                DataRow clusterServerRow = clusterServerTable.NewRow();
                clusterServerRow[0] = serverInfo.Setting.Shard_ID;
                clusterServerRow[1] = serverInfo.Folder;
                clusterServerRow[2] = serverInfo.Setting.Shard_Name;
                clusterServerRow[3] = serverInfo.Setting.Shard_Master;
                clusterServerRow[4] = serverInfo.Setting.Network_ServerPort;
                clusterServerRow[5] = serverInfo.Setting.Steam_AuthPort;
                clusterServerRow[6] = serverInfo.Setting.Steam_MasterPort;
                clusterServerRow[7] = serverInfo.Session;

                clusterServerTable.Rows.Add(clusterServerRow);
            }
        }
        /// <summary>
        /// 将DataGrid转化为属性
        /// </summary>
        /// <param name="columns"></param>
        public void GridToFields()
        {
            for (int i = 0; i < servers.Count; i++)
            {
                DataRow clusterServerRow = clusterServerTable.Rows[i];

                servers[i].Setting.Shard_ID = (string)clusterServerRow[0];
                servers[i].Folder = (string)clusterServerRow[1];
                servers[i].Setting.Shard_Name = (string)clusterServerRow[2];

                servers[i].Setting.Network_ServerPort = int.Parse(clusterServerRow[4].ToString());
                servers[i].Setting.Steam_AuthPort = int.Parse(clusterServerRow[5].ToString());
                servers[i].Setting.Steam_MasterPort = int.Parse(clusterServerRow[6].ToString());
                servers[i].Session = (string)clusterServerRow[7];

                bool temp;
                bool.TryParse((clusterServerRow[3].ToString().ToLower()), out temp);
                servers[i].Setting.Shard_Master = temp;
            }


        }
    }
}
