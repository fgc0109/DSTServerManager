using DSTServerManager.DataHelper;
using DSTServerManager.Saves;
using DSTServerManager.Servers;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DSTServerManager.Servers;

namespace DSTServerManager
{
    public partial class DSTServerLauncher : Window
    {
        #region [本地服务器 服务器列表功能区]----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 本地服务器-服务器文件选择变化
        /// </summary>
        private void dataGrid_LocalServer_ServersPath_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid_LocalServer_ServersPath.SelectedIndex == -1)
            {
                button_LocalServer_DelServer.IsEnabled = false;
                return;
            }
            button_LocalServer_DelServer.IsEnabled = true;

            string path = (dataGrid_LocalServer_ServersPath.SelectedItem as DataRowView)[2].ToString();
            path = path.Replace("bin\\dontstarve_dedicated_server_nullrenderer.exe", "mods");
            UI.Modification.Clear();
            foreach (var item in ServersManager.GetServerModInfo(path))
            {
                UI.Modification.Rows.Add(item.GetItemArray());
            }
        }

        /// <summary>
        /// 本地服务器-添加现有的服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_LocalServer_AddServer_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.Filter = "Server - File(*.exe) | dontstarve_dedicated_server_nullrenderer.exe| 所有文件(*.*) | *.*";
            openFile.ShowDialog();

            if (openFile.SafeFileName.Contains("dontstarve_dedicated_server_nullrenderer"))
            {
                DataRow newPath = UI.ServerLocal.NewRow();
                newPath.ItemArray = new object[3] { 0, "Steam", openFile.FileName };
                UI.ServerLocal.Rows.Add(newPath);
                UI.ServerLocal.RefreshDataTable();
                m_UserDataSQLite.SaveDataTable(UI.ServerLocal, "LocalServerList");
            }
        }

        /// <summary>
        /// 本地服务器-获取新的的服务器
        /// </summary>
        private void button_LocalServer_GetServer_Click(object sender, RoutedEventArgs e)
        {
            if (m_Win_SteamCommand == null) m_Win_SteamCommand = new SteamCommand_1();

            m_Win_SteamCommand.SteamCommandEvent += new SteamCommand_1.SteamCommandHandler(Window_ReceiveLocalCommandValues);
            m_Win_SteamCommand.Show();

            m_Win_SteamCommand.Closed += (object sender2, EventArgs e2) => { m_Win_SteamCommand = null; };
        }
        private void Window_ReceiveLocalCommandValues(object sender, SteamCommandEventArgs commandArgs)
        {
            if (!File.Exists(commandArgs.NewServerPath)) return;

            if (commandArgs.NewServerPath.Contains("dontstarve_dedicated_server_nullrenderer"))
            {
                DataRow newPath = UI.ServerLocal.NewRow();
                newPath.ItemArray = new object[3] { 0, "Steam", commandArgs.NewServerPath };
                UI.ServerLocal.Rows.Add(newPath);
                UI.ServerLocal.RefreshDataTable();
                m_UserDataSQLite.SaveDataTable(UI.ServerLocal, "LocalServerList");
            }
        }

        /// <summary>
        /// 本地服务器-删除指定的服务器
        /// </summary>
        private void button_LocalServer_DelServer_Click(object sender, RoutedEventArgs e)
        {
            int indexPath = dataGrid_LocalServer_ServersPath.SelectedIndex;
            if (indexPath == -1) return;

            UI.ServerLocal.Rows[indexPath].Delete();
            UI.ServerLocal.Rows[indexPath].AcceptChanges();
            UI.ServerLocal.RefreshDataTable();
            m_UserDataSQLite.SaveDataTable(UI.ServerLocal, "LocalServerList");
        }

        #endregion ----------------------------------------------------------------------------------------------------

        #region [本地服务器 集群存档功能区]----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 本地服务器-存档文件夹选择变化
        /// </summary>
        private void ComboBox_LocalServer_SavesFolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //获取集群信息
            string saveFolder = ComboBox_LocalServer_SavesFolder.SelectedItem?.ToString();
            if (saveFolder == string.Empty) return;
            RefreshClusterData(saveFolder, "Cluster", ref listBox_LocalServer_ClusterFile, null);
            m_ClusterInfo_Local = SavesManager.GetClusterInfo(saveFolder, "Cluster");
            if (listBox_LocalServer_ClusterFile.Items.Count != 0) listBox_LocalServer_ClusterFile.SelectedIndex = 0;
        }

        /// <summary>
        /// 本地服务器-集群存档文件夹选择变化
        /// </summary>
        private void listBox_LocalServer_ClusterFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = listBox_LocalServer_ClusterFile.SelectedIndex;
            if (index == -1)
            {
                button_LocalServer_StartCluster.IsEnabled = false;
                return;
            }
            button_LocalServer_StartCluster.IsEnabled = true;

            RefreshServersData(m_ClusterInfo_Local[index], ref UI);
            if (m_ClusterInfo_Local[index].ClusterServers.Count != 0) dataGrid_ClusterInfo_ServersList.SelectedIndex = 0;
        }

        /// <summary>
        /// 本地服务器-添加服务器集群
        /// </summary>       
        private void button_LocalServer_AddCluster_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 本地服务器-刷新当前服务器集群
        /// </summary>
        private void button_LocalServer_RefreshCluster_Click(object sender, RoutedEventArgs e)
        {
            //获取集群信息
            string saveFolder = ComboBox_LocalServer_SavesFolder.SelectedItem?.ToString();
            if (saveFolder == "") return;

            RefreshClusterData(saveFolder, "Cluster", ref listBox_LocalServer_ClusterFile, null);
            m_ClusterInfo_Local = SavesManager.GetClusterInfo("Cluster", saveFolder);
            if (listBox_LocalServer_ClusterFile.Items.Count != 0) listBox_LocalServer_ClusterFile.SelectedIndex = 0;

            int index = listBox_LocalServer_ClusterFile.SelectedIndex;
            if (index == -1) return;

            RefreshServersData(m_ClusterInfo_Local[index], ref UI);
            if (m_ClusterInfo_Local[index].ClusterServers.Count != 0) dataGrid_ClusterInfo_ServersList.SelectedIndex = 0;
        }

        /// <summary>
        /// 本地服务器-开启当前服务器集群
        /// </summary>
        private void button_LocalServer_StartCluster_Click(object sender, RoutedEventArgs e)
        {
            int indexLocalFile = listBox_LocalServer_ClusterFile.SelectedIndex;
            int indexLocalPath = dataGrid_LocalServer_ServersPath.SelectedIndex;

            if (indexLocalFile == -1 || indexLocalPath == -1) return;

            //保存当前选中的集群配置
            ExtendHelper.CopyAllProperties(UI, m_ClusterInfo_Local[indexLocalFile].ClusterSetting);
            SavesManager.SetClusterInfo(ComboBox_LocalServer_SavesFolder.SelectedItem?.ToString(), m_ClusterInfo_Local[indexLocalFile]);

            string confdir = ComboBox_LocalServer_SavesFolder.SelectedItem.ToString();
            string cluster = listBox_LocalServer_ClusterFile.SelectedItem.ToString();
            string exefile = (dataGrid_LocalServer_ServersPath.SelectedItem as DataRowView)[2].ToString();
            bool isShell = (bool)radioButton_LocalServer_OpenType_1.IsChecked;

            //依次开启集群服务器
            foreach (var server in m_ClusterInfo_Local[indexLocalFile].ClusterServers)
            {
                string shard = server.Setting.Shard_Master ? "Master" : "Caves";

                string parameter = ServersManager.CreatParameter(confdir, cluster, shard);
                CreatNewProcess(exefile, parameter, isShell, server.Session);
            }
        }

        #endregion ----------------------------------------------------------------------------------------------------
    }
}
