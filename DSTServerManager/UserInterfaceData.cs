using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using DSTServerManager.Saves;

namespace DSTServerManager
{
    /// <summary>
    /// 界面数据类
    /// 实现INotifyPropertyChanged接口,界面数据变化将会同步改变
    /// </summary>
    class UserInterfaceData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region 集群配置文件字段

        private GameModeEnum gameplay_mode;
        private int gameplay_player;
        private bool gameplay_pvp;
        private bool gameplay_pause;
        private bool gameplay_vote;

        private IntentionEnum network_intention;
        private bool network_lanOnly;
        private bool network_offline;
        private string network_disc;
        private string network_name;
        private string network_pass;
        private int network_tickRate;
        private int network_timeout;

        private bool misc_console;
        private bool misc_mods;

        private bool shard_enabled;
        private string shard_bindIP;
        private string shard_masterIP;
        private int shard_masterPort;
        private string shard_masterKey;

        #endregion

        #region 集群配置文件属性

        public GameModeEnum Gameplay_Mode
        {
            get { return gameplay_mode; }
            set
            {
                Enum.TryParse(value.ToString(), out gameplay_mode);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Gameplay_Mode)));
            }
        }

        public int Gameplay_Player
        {
            get { return gameplay_player; }
            set
            {
                int.TryParse(value.ToString(), out gameplay_player);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Gameplay_Player)));
            }
        }

        public bool Gameplay_PVP
        {
            get { return gameplay_pvp; }
            set
            {
                bool.TryParse(value.ToString(), out gameplay_pvp);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Gameplay_PVP)));
            }
        }

        public bool Gameplay_Pause
        {
            get { return gameplay_pause; }
            set
            {
                bool.TryParse(value.ToString(), out gameplay_pause);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Gameplay_Pause)));
            }
        }

        public bool Gameplay_Vote
        {
            get { return gameplay_vote; }
            set
            {
                bool.TryParse(value.ToString(), out gameplay_vote);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Gameplay_Vote)));
            }
        }

        public IntentionEnum Network_Intention
        {
            get { return network_intention; }
            set
            {
                Enum.TryParse(value.ToString(), out network_intention);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Network_Intention)));
            }
        }

        public bool Network_LanOnly
        {
            get { return network_lanOnly; }
            set
            {
                bool.TryParse(value.ToString(), out network_lanOnly);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Network_LanOnly)));
            }
        }

        public bool Network_Offline
        {
            get { return network_offline; }
            set
            {
                bool.TryParse(value.ToString(), out network_offline);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Network_Offline)));
            }
        }

        public string Network_Disc
        {
            get { return network_disc; }
            set
            {
                network_disc = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Network_Disc)));
            }
        }

        public string Network_Name
        {
            get { return network_name; }
            set
            {
                network_name = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Network_Name)));
            }
        }

        public string Network_Pass
        {
            get { return network_pass; }
            set
            {
                network_pass = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Network_Pass)));
            }
        }

        public int Network_TickRate
        {
            get { return network_tickRate; }
            set
            {
                int.TryParse(value.ToString(), out network_tickRate);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Network_TickRate)));
            }
        }

        public int Network_Timeout
        {
            get { return network_timeout; }
            set
            {
                int.TryParse(value.ToString(), out network_timeout);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Network_Timeout)));
            }
        }

        public bool Misc_Console
        {
            get { return misc_console; }
            set
            {
                bool.TryParse(value.ToString(), out misc_console);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Misc_Console)));
            }
        }

        public bool Misc_Mods
        {
            get { return misc_mods; }
            set
            {
                bool.TryParse(value.ToString(), out misc_mods);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Misc_Mods)));
            }
        }

        /// <summary>
        /// 分片设置 开启集群分片功能
        /// </summary>
        public bool Shard_Enabled
        {
            get { return shard_enabled; }
            set
            {
                bool.TryParse(value.ToString(), out shard_enabled);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Shard_Enabled)));
            }
        }

        /// <summary>
        /// 分片设置 绑定用于监听玩家连接事件的IP地址
        /// [单物理服务器使用默认配置 127.0.0.1 ]
        /// [多物理服务器必须配置为 0.0.0.0 ]
        /// </summary>
        public string Shard_BindIP
        {
            get { return shard_bindIP; }
            set
            {
                shard_bindIP = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Shard_BindIP)));
            }
        }

        /// <summary>
        /// 分片设置 主服务器IP地址
        /// [单物理服务器配置为 127.0.0.1 ]
        /// [多物理服务器配置为对应的主服务器广域网IP地址]
        /// </summary>
        public string Shard_MasterIP
        {
            get { return shard_masterIP; }
            set
            {
                shard_masterIP = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Shard_MasterIP)));
            }
        }

        /// <summary>
        /// 分片设置 主服务器端口
        /// </summary>
        public int Shard_MasterPort
        {
            get { return shard_masterPort; }
            set
            {
                int.TryParse(value.ToString(), out shard_masterPort);
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Shard_MasterPort)));
            }
        }

        /// <summary>
        /// 分片设置 集群主服务器密钥
        /// [集群隐私措施 主服务器将会拒绝不能提供该密钥的服务器加入集群]
        /// </summary>
        public string Shard_MasterKey
        {
            get { return shard_masterKey; }
            set
            {
                shard_masterKey = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Shard_MasterKey)));
            }
        }

        #endregion

        #region 服务器配置文件字段

        private int network_serverPort;

        private bool shard_master;
        private string shard_name;
        private string shard_id;

        private int steam_masterPort;
        private int steam_authPort;

        #endregion

        #region 服务器配置文件属性

        public int Network_ServerPort
        {
            get { return network_serverPort; }
            set
            {
                network_serverPort = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Network_ServerPort)));
            }
        }

        public bool Shard_Master
        {
            get { return shard_master; }
            set
            {
                shard_master = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Shard_Master)));
            }
        }

        public string Shard_Name
        {
            get { return shard_name; }
            set
            {
                shard_name = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Shard_Name)));
            }
        }

        public string Shard_ID
        {
            get { return shard_id; }
            set
            {
                shard_id = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Shard_ID)));
            }
        }

        /// <summary>
        /// port used by steam.Make sure that this is different for each server you run on the same machine.
        /// </summary>
        public int Steam_MasterPort
        {
            get { return steam_masterPort; }
            set
            {
                steam_masterPort = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Steam_MasterPort)));
            }
        }

        /// <summary>
        /// port used by steam.Make sure that this is different for each server you run on the same machine.
        /// </summary>
        public int Steam_AuthPort
        {
            get { return steam_authPort; }
            set
            {
                steam_authPort = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Steam_AuthPort)));
            }
        }

        #endregion

        #region 集群信息字段

        private List<ServerInfo> clusterServers = new List<ServerInfo>();

        //集群信息种不包含DataTable的对应属性,该属性仅用于界面显示
        private DataTable clusterServersTable = new DataTable();

        #endregion

        #region 集群信息属性

        public List<ServerInfo> ClusterServers
        {
            get
            {
                GridToFields();
                return clusterServers;
            }
            set
            {
                clusterServers = value;
                FieldsToGrid();
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ClusterServers)));
            }
        }

        public DataTable ClusterServersTable
        {
            get
            {
                FieldsToGrid();
                return clusterServersTable;
            }
            set
            {
                clusterServersTable = value;
                GridToFields();
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ClusterServersTable)));
            }
        }

        #endregion

        #region 用户信息字段

        private DataTable serverFileListTable_Local = new DataTable("serverFileListTable_Local");
        private DataTable serverFileListTable_Cloud = new DataTable("serverFileListTable_Cloud");
        private DataTable serverConnectsTable_Cloud = new DataTable("serverConnectsTable_Cloud");
        private DataTable serverConsole = new DataTable("serverConsole");
        private DataTable serverLeveled = new DataTable("serverLeveled");

        private List<string> saveFolders_Local = new List<string>();
        private List<string> saveFolders_Cloud = new List<string>();

        #endregion

        #region 用户信息属性

        public List<string> SaveFolders_Local
        {
            get { return saveFolders_Local; }
            set
            {
                saveFolders_Local = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(SaveFolders_Local)));
            }
        }

        public List<string> SaveFolders_Cloud
        {
            get { return saveFolders_Cloud; }
            set
            {
                saveFolders_Cloud = value;
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(SaveFolders_Cloud)));
            }
        }

        public DataTable ServerFileListTable_Local
        {
            get { return serverFileListTable_Local; }
            set
            {
                serverFileListTable_Local = value.Copy();
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ServerFileListTable_Local)));
            }
        }
        public DataTable ServerFileListTable_Cloud
        {
            get { return serverFileListTable_Cloud; }
            set
            {
                serverFileListTable_Cloud = value.Copy();
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ServerFileListTable_Cloud)));
            }
        }
        public DataTable ServerConnectsTable_Cloud
        {
            get { return serverConnectsTable_Cloud; }
            set
            {
                serverConnectsTable_Cloud = value.Copy();
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ServerConnectsTable_Cloud)));
            }
        }
        public DataTable ServerConsole
        {
            get { return serverConsole; }
            set
            {
                serverConsole = value.Copy();
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ServerConsole)));
            }
        }
        public DataTable ClusterServersLevel
        {
            get { return serverLeveled; }
            set
            {
                serverLeveled = value.Copy();
                if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ClusterServersLevel)));
            }
        }

        #endregion

        #region 用户操作字段

        private string location;
        private string username;
        private string password;

        #endregion

        #region 用户操作字段

        public DataRowView Connection
        {
            set
            {
                location = (value as DataRowView)[1].ToString();
                username = (value as DataRowView)[2].ToString();
                password = (value as DataRowView)[3].ToString();
                if (PropertyChanged == null) return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Location)));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Username)));
            }
        }

        public string Location { get { return location; } }
        public string Username { get { return username; } }
        public string Password { get { return password; } }

        #endregion

        public UserInterfaceData(int columns)
        {
            for (int i = 0; i < columns; i++)
                clusterServersTable.Columns.Add("Column_" + i.ToString());
        }

        /// <summary>
        /// 按照界面对DataTable的格式需求
        /// 将属性转化为DataTable
        /// </summary>
        /// <param name="columns"></param>
        public void FieldsToGrid()
        {
            foreach (var serverInfo in clusterServers)
            {
                DataRow clusterServerRow = clusterServersTable.NewRow();
                clusterServerRow[0] = serverInfo.Setting.Shard_ID;
                clusterServerRow[1] = serverInfo.Folder;
                clusterServerRow[2] = serverInfo.Setting.Shard_Name;
                clusterServerRow[3] = serverInfo.Setting.Shard_Master;
                clusterServerRow[4] = serverInfo.Setting.Network_ServerPort;
                clusterServerRow[5] = serverInfo.Setting.Steam_AuthPort;
                clusterServerRow[6] = serverInfo.Setting.Steam_MasterPort;
                clusterServerRow[7] = serverInfo.Session;

                clusterServersTable.Rows.Add(clusterServerRow);
            }
        }

        /// <summary>
        /// 按照界面对DataTable的格式需求
        /// 将DataTable转化为属性
        /// </summary>
        /// <param name="columns"></param>
        public void GridToFields()
        {
            for (int i = 0; i < clusterServers.Count; i++)
            {
                DataRow clusterServerRow = clusterServersTable.Rows[i];

                clusterServers[i].Setting.Shard_ID = (string)clusterServerRow[0];
                clusterServers[i].Folder = (string)clusterServerRow[1];
                clusterServers[i].Setting.Shard_Name = (string)clusterServerRow[2];

                clusterServers[i].Setting.Network_ServerPort = int.Parse(clusterServerRow[4].ToString());
                clusterServers[i].Setting.Steam_AuthPort = int.Parse(clusterServerRow[5].ToString());
                clusterServers[i].Setting.Steam_MasterPort = int.Parse(clusterServerRow[6].ToString());
                clusterServers[i].Session = (string)clusterServerRow[7];

                bool temp;
                bool.TryParse((clusterServerRow[3].ToString().ToLower()), out temp);
                clusterServers[i].Setting.Shard_Master = temp;
            }
        }
    }
}
