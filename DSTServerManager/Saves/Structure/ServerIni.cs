using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSTServerManager.DataHelper;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;
using Renci.SshNet;
using System.IO;

namespace DSTServerManager.Saves
{
    /// <summary>
    /// 服务器集群配置文件
    /// </summary>
    class ServerIni
    {
        private IniHelper m_Setting = null;

        #region 配置文件字段

        private int network_serverPort;

        private bool shard_master;
        private string shard_name;
        private string shard_id;

        private int steam_masterPort;
        private int steam_authPort;

        #endregion

        #region 配置文件属性

        public int Network_ServerPort
        {
            get { return network_serverPort; }
            set { network_serverPort = value; }
        }

        public bool Shard_Master
        {
            get { return shard_master; }
            set { shard_master = value; }
        }

        public string Shard_Name
        {
            get { return shard_name; }
            set { shard_name = value; }
        }

        public string Shard_ID
        {
            get { return shard_id; }
            set { shard_id = value; }
        }

        /// <summary>
        /// port used by steam.Make sure that this is different for each server you run on the same machine.
        /// </summary>
        public int Steam_MasterPort
        {
            get { return steam_masterPort; }
            set { steam_masterPort = value; }
        }

        /// <summary>
        /// port used by steam.Make sure that this is different for each server you run on the same machine.
        /// </summary>
        public int Steam_AuthPort
        {
            get { return steam_authPort; }
            set { steam_authPort = value; }
        }

        #endregion

        /// <summary>
        /// 默认的配置文件
        /// </summary>
        public ServerIni()
        {
            network_serverPort = 10000;

            shard_master = false;
            shard_name = "null";
            shard_id = "0000000";

            steam_masterPort = 21000;
            steam_authPort = 22000;
        }

        /// <summary>
        /// 从文件读取配置文件
        /// </summary>
        /// <param name="serverIniFullPath"></param>
        public void ReadFromFile(string serverIniFullPath)
        {
            if (!File.Exists(serverIniFullPath)) return;

            MemoryStream serverDataStream = new MemoryStream(File.ReadAllBytes(serverIniFullPath));
            m_Setting = new IniHelper(serverDataStream, false);
            serverDataStream.Close();

            try { SettingToFields(); }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// 写入位于本地配置文件
        /// </summary>
        /// <param name="serverIniFullPath"></param>
        public void WriteToFile(string serverIniFullPath)
        {
            if (null == m_Setting) return;
            try { FieldsToSetting(); }
            catch (Exception) { throw; }

            MemoryStream serverDataStream = m_Setting.GetIniStream();
            FileStream clusterFileStream = new FileStream(serverIniFullPath, FileMode.Create);
            BinaryWriter w = new BinaryWriter(clusterFileStream);
            w.Write(serverDataStream.ToArray());
            clusterFileStream.Close();
            serverDataStream.Close();
        }

        /// <summary>
        /// 写入位于SSH服务器配置文件
        /// </summary>
        /// <param name="serverIniFullPath"></param>
        public void ReadFromSSH(string serverIniFullPath, SftpClient client)
        {
            MemoryStream stream = new MemoryStream();
            client.OpenRead(serverIniFullPath).CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin);

            m_Setting = new IniHelper(stream, false);
            try { SettingToFields(); }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// 内部函数 将已经读取的INI文件参数转换为类属性
        /// </summary>
        private void SettingToFields()
        {
            network_serverPort = int.Parse(m_Setting.ReadIniData("NETWORK", "server_port", network_serverPort.ToString()));

            shard_master = bool.Parse(m_Setting.ReadIniData("SHARD", "is_master", shard_master.ToString()));
            shard_name = m_Setting.ReadIniData("SHARD", "name", shard_name.ToString());
            shard_id = m_Setting.ReadIniData("SHARD", "id", shard_id.ToString());

            steam_masterPort = int.Parse(m_Setting.ReadIniData("STEAM", "master_server_port", steam_masterPort.ToString()));
            steam_authPort = int.Parse(m_Setting.ReadIniData("STEAM", "authentication_port", steam_authPort.ToString()));
        }

        /// <summary>
        /// 内部函数 将类属性转换为INI文件参数
        /// </summary>
        private void FieldsToSetting()
        {
            m_Setting.WriteIniData("NETWORK", "server_port", network_serverPort.ToString());

            m_Setting.WriteIniData("SHARD", "is_master", shard_master.ToString());
            m_Setting.WriteIniData("SHARD", "name", shard_name.ToString());
            m_Setting.WriteIniData("SHARD", "id", shard_id.ToString());

            m_Setting.WriteIniData("STEAM", "master_server_port", steam_masterPort.ToString());
            m_Setting.WriteIniData("STEAM", "authentication_port", steam_authPort.ToString());
        }
    }
}
