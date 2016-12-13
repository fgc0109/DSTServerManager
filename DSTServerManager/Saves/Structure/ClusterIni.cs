using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSTServerManager.DataHelper;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;
using Renci.SshNet;

namespace DSTServerManager.Saves
{
    /// <summary>
    /// 服务器集群配置文件
    /// </summary>
    class ClusterIni
    {
        private IniHelper m_Setting = null;

        #region 配置文件字段

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

        #region 配置文件属性

        public GameModeEnum Gameplay_Mode
        {
            get { return gameplay_mode; }
            set { Enum.TryParse(value.ToString(), out gameplay_mode); }
        }

        public int Gameplay_Player
        {
            get { return gameplay_player; }
            set { int.TryParse(value.ToString(), out gameplay_player); }
        }

        public bool Gameplay_PVP
        {
            get { return gameplay_pvp; }
            set { bool.TryParse(value.ToString(), out gameplay_pvp); }
        }

        public bool Gameplay_Pause
        {
            get { return gameplay_pause; }
            set { bool.TryParse(value.ToString(), out gameplay_pause); }
        }

        public bool Gameplay_Vote
        {
            get { return gameplay_vote; }
            set { bool.TryParse(value.ToString(), out gameplay_vote); }
        }

        public IntentionEnum Network_Intention
        {
            get { return network_intention; }
            set { Enum.TryParse(value.ToString(), out network_intention); }
        }

        public bool Network_LanOnly
        {
            get { return network_lanOnly; }
            set { bool.TryParse(value.ToString(), out network_lanOnly); }
        }

        public bool Network_Offline
        {
            get { return network_offline; }
            set { bool.TryParse(value.ToString(), out network_offline); }
        }

        public string Network_Disc
        {
            get { return network_disc; }
            set { network_disc = value; }
        }

        public string Network_Name
        {
            get { return network_name; }
            set { network_name = value; }
        }

        public string Network_Pass
        {
            get { return network_pass; }
            set { network_pass = value; }
        }

        public int Network_TickRate
        {
            get { return network_tickRate; }
            set { int.TryParse(value.ToString(), out network_tickRate); }
        }

        public int Network_Timeout
        {
            get { return network_timeout; }
            set { int.TryParse(value.ToString(), out network_timeout); }
        }

        public bool Misc_Console
        {
            get { return misc_console; }
            set { bool.TryParse(value.ToString(), out misc_console); }
        }

        public bool Misc_Mods
        {
            get { return misc_mods; }
            set { bool.TryParse(value.ToString(), out misc_mods); }
        }

        /// <summary>
        /// 分片设置 开启集群分片功能
        /// </summary>
        public bool Shard_Enabled
        {
            get { return shard_enabled; }
            set { bool.TryParse(value.ToString(), out shard_enabled); }
        }

        /// <summary>
        /// 分片设置 绑定用于监听玩家连接事件的IP地址
        /// [单物理服务器使用默认配置 127.0.0.1 ]
        /// [多物理服务器必须配置为 0.0.0.0 ]
        /// </summary>
        public string Shard_BindIP
        {
            get { return shard_bindIP; }
            set { shard_bindIP = value; }
        }

        /// <summary>
        /// 分片设置 主服务器IP地址
        /// [单物理服务器配置为 127.0.0.1 ]
        /// [多物理服务器配置为对应的主服务器广域网IP地址]
        /// </summary>
        public string Shard_MasterIP
        {
            get { return shard_masterIP; }
            set { shard_masterIP = value; }
        }

        /// <summary>
        /// 分片设置 主服务器端口
        /// </summary>
        public int Shard_MasterPort
        {
            get { return shard_masterPort; }
            set { int.TryParse(value.ToString(), out shard_masterPort); }
        }

        /// <summary>
        /// 分片设置 集群主服务器密钥
        /// [集群隐私措施 主服务器将会拒绝不能提供该密钥的服务器加入集群]
        /// </summary>
        public string Shard_MasterKey
        {
            get { return shard_masterKey; }
            set { shard_masterKey = value; }
        }

        #endregion

        /// <summary>
        /// 默认的配置文件
        /// </summary>
        public ClusterIni()
        {
            gameplay_mode = GameModeEnum.survival;
            gameplay_player = 6;
            gameplay_pvp = false;
            gameplay_pause = true;
            gameplay_vote = false;

            network_intention = IntentionEnum.cooperative;
            network_lanOnly = false;
            network_offline = false;
            network_disc = "";
            network_name = "Default";
            network_pass = "";
            network_tickRate = 60;
            network_timeout = 8000;

            misc_console = true;
            misc_mods = true;

            shard_enabled = true;
            shard_bindIP = "127.0.0.1";
            shard_masterIP = "127.0.0.1";
            shard_masterPort = 10000;
            shard_masterKey = "defaultPass";
        }

        /// <summary>
        /// 从文件读取配置文件
        /// </summary>
        /// <param name="clusterIniFullPath">集群配置文件完整路径</param>
        public void ReadFromFile(string clusterIniFullPath)
        {
            if (!File.Exists(clusterIniFullPath)) return;

            MemoryStream clusterDataStream = new MemoryStream(File.ReadAllBytes(clusterIniFullPath));
            m_Setting = new IniHelper(clusterDataStream, false);
            clusterDataStream.Close();

            try { SettingToFields(); }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// 写入位于本地的Cluster配置文件
        /// </summary>
        /// <param name="clusterIniFullPath">集群配置文件完整路径</param>
        public void WriteToFile(string clusterIniFullPath)
        {
            if (null == m_Setting) return;
            try { FieldsToSetting(); }
            catch (Exception) { throw; }

            MemoryStream clusterDataStream = m_Setting.GetIniStream();
            FileStream clusterFileStream = new FileStream(clusterIniFullPath, FileMode.Create);
            BinaryWriter write = new BinaryWriter(clusterFileStream);
            write.Write(clusterDataStream.ToArray());
            clusterFileStream.Close();
            clusterDataStream.Close();
        }

        /// <summary>
        /// 读取位于SSH服务器的Cluster配置文件
        /// </summary>
        /// <param name="serverIniFullPath"></param>
        public void ReadFromSSH(string clusterIniFullPath, SftpClient client)
        {
            MemoryStream stream = new MemoryStream();
            client.OpenRead(clusterIniFullPath).CopyTo(stream);

            m_Setting = new IniHelper(stream, false);
            try { SettingToFields(); }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// 写入位于SSH服务器的Cluster配置文件
        /// </summary>
        /// <param name="clusterIniFullPath"></param>
        public void WriteToSSH(string clusterIniFullPath, SftpClient client)
        {
            if (null == m_Setting) return;
            try { FieldsToSetting(); }
            catch (Exception) { throw; }

            MemoryStream clusterDataStream = m_Setting.GetIniStream();
            client.WriteAllBytes(clusterIniFullPath, clusterDataStream.ToArray());
            clusterDataStream.Close();
        }

        /// <summary>
        /// 内部函数 将已经读取的INI文件参数转换为类属性
        /// </summary>
        private void SettingToFields()
        {
            gameplay_mode = (GameModeEnum)Enum.Parse(typeof(GameModeEnum), m_Setting.ReadIniData("GAMEPLAY", "game_mode", gameplay_mode.ToString()));
            gameplay_player = int.Parse(m_Setting.ReadIniData("GAMEPLAY", "max_players", gameplay_player.ToString()));
            gameplay_pvp = bool.Parse(m_Setting.ReadIniData("GAMEPLAY", "pvp", gameplay_pvp.ToString()));
            gameplay_pause = bool.Parse(m_Setting.ReadIniData("GAMEPLAY", "pause_when_empty", gameplay_pause.ToString()));
            gameplay_vote = bool.Parse(m_Setting.ReadIniData("GAMEPLAY", "vote_enabled", gameplay_vote.ToString()));

            network_intention = (IntentionEnum)Enum.Parse(typeof(IntentionEnum), m_Setting.ReadIniData("NETWORK", "cluster_intention", network_intention.ToString()));
            network_lanOnly = bool.Parse(m_Setting.ReadIniData("NETWORK", "lan_only_cluster", network_lanOnly.ToString()));
            network_offline = bool.Parse(m_Setting.ReadIniData("NETWORK", "offline_cluster", network_offline.ToString()));
            network_disc = m_Setting.ReadIniData("NETWORK", "cluster_description", network_disc.ToString());
            network_name = m_Setting.ReadIniData("NETWORK", "cluster_name", network_name.ToString());
            network_pass = m_Setting.ReadIniData("NETWORK", "cluster_password", network_pass.ToString());
            network_tickRate = int.Parse(m_Setting.ReadIniData("NETWORK", "tick_rate", network_tickRate.ToString()));
            network_timeout = int.Parse(m_Setting.ReadIniData("NETWORK", "connection_timeout", network_timeout.ToString()));

            misc_console = bool.Parse(m_Setting.ReadIniData("MISC", "console_enabled", misc_console.ToString()));
            misc_mods = bool.Parse(m_Setting.ReadIniData("MISC", "mods_enabled", misc_mods.ToString()));

            shard_enabled = bool.Parse(m_Setting.ReadIniData("SHARD", "shard_enabled", shard_enabled.ToString()));
            shard_bindIP = m_Setting.ReadIniData("SHARD", "bind_ip", shard_bindIP.ToString());
            shard_masterIP = m_Setting.ReadIniData("SHARD", "master_ip", shard_masterIP.ToString());
            shard_masterPort = int.Parse(m_Setting.ReadIniData("SHARD", "master_port", shard_masterPort.ToString()));
            shard_masterKey = m_Setting.ReadIniData("SHARD", "cluster_key", shard_masterKey.ToString());
        }

        /// <summary>
        /// 内部函数 将类属性转换为INI文件参数
        /// </summary>
        private void FieldsToSetting()
        {
            m_Setting.WriteIniData("GAMEPLAY", "game_mode", gameplay_mode.ToString());
            m_Setting.WriteIniData("GAMEPLAY", "max_players", gameplay_player.ToString());
            m_Setting.WriteIniData("GAMEPLAY", "pvp", gameplay_pvp.ToString());
            m_Setting.WriteIniData("GAMEPLAY", "pause_when_empty", gameplay_pause.ToString());
            m_Setting.WriteIniData("GAMEPLAY", "vote_enabled", gameplay_vote.ToString());

            m_Setting.WriteIniData("NETWORK", "cluster_intention", network_intention.ToString());
            m_Setting.WriteIniData("NETWORK", "lan_only_cluster", network_lanOnly.ToString());
            m_Setting.WriteIniData("NETWORK", "offline_cluster", network_offline.ToString());
            m_Setting.WriteIniData("NETWORK", "cluster_description", network_disc.ToString());
            m_Setting.WriteIniData("NETWORK", "cluster_name", network_name.ToString());
            m_Setting.WriteIniData("NETWORK", "cluster_password", network_pass.ToString());
            m_Setting.WriteIniData("NETWORK", "tick_rate", network_tickRate.ToString());
            m_Setting.WriteIniData("NETWORK", "connection_timeout", network_timeout.ToString());

            //他妈的,研究了两天,最后发现这个傻逼的选项不管改成什么控制台都输不进去命令,去掉就好了
            m_Setting.DeleteIniData("MISC", "console_enabled");
            //m_Setting.WriteIniData("MISC", "console_enabled", misc_console.ToString());
            m_Setting.WriteIniData("MISC", "mods_enabled", misc_mods.ToString());

            m_Setting.WriteIniData("SHARD", "shard_enabled", shard_enabled.ToString());
            m_Setting.WriteIniData("SHARD", "bind_ip", shard_bindIP.ToString());
            m_Setting.WriteIniData("SHARD", "master_ip", shard_masterIP.ToString());
            m_Setting.WriteIniData("SHARD", "master_port", shard_masterPort.ToString());
            m_Setting.WriteIniData("SHARD", "cluster_key", shard_masterKey.ToString());
        }
    }

    /// <summary>
    /// 游戏模式
    /// </summary>
    public enum GameModeEnum
    {
        survival,                                   //无尽模式
        endless,                                    //生存模式
        wilderness                                  //荒野模式
    }

    /// <summary>
    /// 游戏意向
    /// </summary>
    public enum IntentionEnum
    {
        cooperative,                                //合作模式
        social,                                     //
        competitive,                                //
        madness                                     //
    }
}
