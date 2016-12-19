using DSTServerManager.DataHelper;
using DSTServerManager.Saves;
using DSTServerManager.Servers;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Renci.SshNet.Common;

namespace DSTServerManager
{
    public partial class DSTServerLauncher : Window
    {
        #region [远程服务器 服务器连接功能区]----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 远程服务器-打开远程连接
        /// </summary>
        private void dataGrid_CloudServer_Connection_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CreatNewConnect(UI.Location, UI.Username, UI.Password);
        }

        private void dataGrid_CloudServer_Connection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int indexConn = dataGrid_CloudServer_Connections.SelectedIndex;
            if (indexConn == -1) return;

            List<string> serverID = new List<string>();
            foreach (var server in UI.ServerConnectsTable_Cloud.DefaultView[indexConn][4].ToString().Split('|'))
                serverID.Add(server.ToString());
            UI.ServerFileListTable_Cloud = ServerFileListTable_CloudOrigin.CopyConfirmTable(0, serverID);
        }

        /// <summary>
        /// 增加远程服务器链接
        /// </summary>
        private void button_CloudServer_AddConn_Click(object sender, RoutedEventArgs e)
        {
            DataRow currentRow = UI.ServerConnectsTable_Cloud.NewRow();
            int newIndex = UI.ServerConnectsTable_Cloud.Rows.Count + 1;

            if (m_Win_CloudConnection == null) m_Win_CloudConnection = new CloudConnection(currentRow, true, newIndex);

            m_Win_CloudConnection.CloudConnectionEvent += new CloudConnection.CloudConnectionHandler(window_ReceiveConnectionValues);
            m_Win_CloudConnection.Show();

            m_Win_CloudConnection.Closed += (object sender2, EventArgs e2) => { m_Win_CloudConnection = null; };
        }

        /// <summary>
        /// 编辑当前选中的远程服务器链接
        /// </summary>
        private void button_CloudServer_EditConn_Click(object sender, RoutedEventArgs e)
        {
            int indexConn = dataGrid_CloudServer_Connections.SelectedIndex;
            if (indexConn == -1) return;

            DataRow currentRow = UI.ServerConnectsTable_Cloud.Rows[indexConn];
            if (m_Win_CloudConnection == null) m_Win_CloudConnection = new CloudConnection(currentRow, false, 0);

            m_Win_CloudConnection.CloudConnectionEvent += new CloudConnection.CloudConnectionHandler(window_ReceiveConnectionValues);
            m_Win_CloudConnection.Show();

            m_Win_CloudConnection.Closed += (object sender2, EventArgs e2) => { m_Win_CloudConnection = null; };
        }

        /// <summary>
        /// 删除当前选中的远程服务器链接
        /// </summary>
        private void button_CloudServer_DeleteConn_Click(object sender, RoutedEventArgs e)
        {
            int indexConn = dataGrid_CloudServer_Connections.SelectedIndex;
            if (indexConn == -1) return;

            UI.ServerConnectsTable_Cloud.Rows[indexConn].Delete();
            UI.ServerConnectsTable_Cloud.AcceptChanges();

            UI.ServerConnectsTable_Cloud.RefreshDataTable();
            m_UserDataSQLite.SaveDataTable(UI.ServerConnectsTable_Cloud, "CloudServerConnList");
        }

        /// <summary>
        /// 子窗口数据传递事件
        /// </summary>
        private void window_ReceiveConnectionValues(object sender, CloudConnectionEventArgs connectionArgs)
        {
            if (connectionArgs.IsNewRow)
            {
                UI.ServerConnectsTable_Cloud.Rows.Add(connectionArgs.GetRow);

                UI.ServerConnectsTable_Cloud.RefreshDataTable();
                m_UserDataSQLite.SaveDataTable(UI.ServerConnectsTable_Cloud, "CloudServerConnList");
            }
            else
            {
                m_UserDataSQLite.UpdateDataTable(UI.ServerConnectsTable_Cloud, "CloudServerConnList");
            }
        }

        #endregion ----------------------------------------------------------------------------------------------------

        #region [远程服务器 服务器列表功能区]----------------------------------------------------------------------------------------------------


        private void Button_CloudServer_AddServer_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.Filter = "EXE - File(*.exe) | *.exe| 所有文件(*.*) | *.*";
            openFile.ShowDialog();
        }


        private void Button_CloudServer_GetServer_Click(object sender, RoutedEventArgs e)
        {
            if (m_Win_CloudCommand == null) m_Win_CloudCommand = new SteamCommand_2(UI.Location, UI.Username, UI.Password);

            m_Win_CloudCommand.SteamCommandEvent += new SteamCommand_2.SteamCommandHandler(Window_ReceiveCloudCommandValues);
            m_Win_CloudCommand.Show();

            m_Win_CloudCommand.Closed += (object sender2, EventArgs e2) => { m_Win_SteamCommand = null; };
        }

        private void Window_ReceiveCloudCommandValues(object sender, SteamCommandEventArgs commandArgs)
        {
            //if (!File.Exists(commandArgs.NewServerPath)) return;
            int indexConn = dataGrid_CloudServer_Connections.SelectedIndex;

            if (commandArgs.NewServerPath.Contains("dontstarve_dedicated_server_nullrenderer"))
            {
                DataRow newPath = UI.ServerFileListTable_Cloud.NewRow();
                newPath.ItemArray = new object[3] { 0, "Steam", commandArgs.NewServerPath };
                UI.ServerFileListTable_Cloud.Rows.Add(newPath);
                UI.ServerFileListTable_Cloud.RefreshDataTable();
                m_UserDataSQLite.SaveDataTable(UI.ServerFileListTable_Cloud, "CloudServerList");

                //需要查找远程服务器链接列表整合后的列表ID
                UI.ServerConnectsTable_Cloud.DefaultView[indexConn][4] = 1;
            }
        }


        #endregion ----------------------------------------------------------------------------------------------------

        #region [远程服务器 集群存档功能区]----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 远程服务器-存档文件夹选择变化
        /// </summary>
        private void ComboBox_CloudServer_SavesFolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SftpClient client = ServersManager.GetExistSftp(m_ServerConnect, UI.Location, UI.Username, UI.Password);
            if (client == null) return;

            //获取集群信息
            string saveFolder = ComboBox_CloudServer_SavesFolder.SelectedItem?.ToString();
            if (saveFolder == string.Empty) return;
            RefreshClusterData(saveFolder, "Cluster", ref listBox_CloudServer_ClusterFile, client);
            m_ClusterInfo_Cloud = SavesManager.GetClusterInfo(saveFolder, "Cluster", client);
            if (listBox_CloudServer_ClusterFile.Items.Count != 0) listBox_CloudServer_ClusterFile.SelectedIndex = 0;
        }

        /// <summary>
        /// 远程服务器-集群存档文件夹选择变化
        /// </summary>
        private void listBox_CloudServer_ClusterFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = listBox_CloudServer_ClusterFile.SelectedIndex;
            if (index == -1) return;
            RefreshServersData(m_ClusterInfo_Cloud[index], ref UI);
            if (m_ClusterInfo_Cloud[index].ClusterServers.Count != 0) dataGrid_ClusterInfo_ServersList.SelectedIndex = 0;
        }

        private void textBox_CloudServer_ClusterSaveList_AddPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UI == null) return;
            button_CloudServer_AddCluster.IsEnabled = false;

            SftpClient client = ServersManager.GetExistSftp(m_ServerConnect, UI.Location, UI.Username, UI.Password);
            if (client == null) return;
            if (UI.SaveFolders_Cloud.Contains(textBox_CloudServer_ClusterSaveList_AddPath.Text)) return;

            button_CloudServer_AddCluster.IsEnabled = true;
        }

        private void button_CloudServer_AddCluster_Click(object sender, RoutedEventArgs e)
        {
            SftpClient client = ServersManager.GetExistSftp(m_ServerConnect, UI.Location, UI.Username, UI.Password);
            if (client == null) return;

            string userPath = (UI.Username == "root") ? "/root" : $"/home/{UI.Username}";
            try { client.CreateDirectory($"{userPath}/.klei/{textBox_CloudServer_ClusterSaveList_AddPath.Text}"); }
            catch (SshException) { }
            catch (Exception) { throw; };

            UI.SaveFolders_Cloud.Add(textBox_CloudServer_ClusterSaveList_AddPath.Text);
        }

        private void button_CloudServer_StartCluster_Click(object sender, RoutedEventArgs e)
        {
            SftpClient client = ServersManager.GetExistSftp(m_ServerConnect, UI.Location, UI.Username, UI.Password);
            string nameCloud = ComboBox_CloudServer_SavesFolder.SelectedItem?.ToString();
            int indexCloud = listBox_CloudServer_ClusterFile.SelectedIndex;
            int indexCloudFile = listBox_CloudServer_ClusterFile.SelectedIndex;

            if (client == null) return;

            ExtendHelper.CopyAllProperties(UI, m_ClusterInfo_Cloud[indexCloud].ClusterSetting);
            SavesManager.SetClusterInfo(nameCloud, m_ClusterInfo_Cloud[indexCloud], client);

            string confdir = ComboBox_CloudServer_SavesFolder.SelectedItem.ToString();
            string cluster = listBox_CloudServer_ClusterFile.SelectedItem.ToString();
            string exefile = (dataGrid_CloudServer_ServersPath.SelectedItem as DataRowView)[2].ToString();
            string exepath = exefile.Replace("/dontstarve_dedicated_server_nullrenderer", "");

            List<string> screenList = ServersManager.GetExistScreens(UI.Location, UI.Username, UI.Password);

            foreach (var server in m_ClusterInfo_Cloud[indexCloudFile].ClusterServers)
            {
                string shard = server.Setting.Shard_Master ? "Master" : "Caves";

                string parameter = ServersManager.CreatParameter(confdir, cluster, shard);

                string command = string.Empty;
                string screenName = cluster + "_" + server.Folder;

                if (screenList.Contains(screenName))
                {
                    command = $"screen -xr {screenName}";
                }
                else
                {
                    command += $"cd {exepath}\r";
                    command += $"screen -S {screenName} {"./dontstarve_dedicated_server_nullrenderer"} {parameter}";
                }

                CreatNewScreens(UI.Location, UI.Username, UI.Password, command);
            }
        }

        #endregion ----------------------------------------------------------------------------------------------------
    }
}
