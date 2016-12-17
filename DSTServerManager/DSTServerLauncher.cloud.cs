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
            string location = (dataGrid_CloudServer_Connections.SelectedItem as DataRowView)[1].ToString();
            string username = (dataGrid_CloudServer_Connections.SelectedItem as DataRowView)[2].ToString();
            string password = (dataGrid_CloudServer_Connections.SelectedItem as DataRowView)[3].ToString();

            CreatNewConnect(location, username, password);
        }

        private void dataGrid_CloudServer_Connection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int indexConn = dataGrid_CloudServer_Connections.SelectedIndex;
            if (indexConn == -1) return;

            List<string> serverID = new List<string>();
            foreach (var server in UI_DATA.ServerConnectsTable_Cloud.DefaultView[indexConn][4].ToString().Split('|'))
                serverID.Add(server.ToString());
            UI_DATA.ServerFileListTable_Cloud = ServerFileListTable_CloudOrigin.CopyConfirmTable(0, serverID);
        }

        /// <summary>
        /// 增加远程服务器链接
        /// </summary>
        private void button_CloudServer_AddConn_Click(object sender, RoutedEventArgs e)
        {
            DataRow currentRow = UI_DATA.ServerConnectsTable_Cloud.NewRow();
            int newIndex = UI_DATA.ServerConnectsTable_Cloud.Rows.Count + 1;

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

            DataRow currentRow = UI_DATA.ServerConnectsTable_Cloud.Rows[indexConn];
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

            UI_DATA.ServerConnectsTable_Cloud.Rows[indexConn].Delete();
            UI_DATA.ServerConnectsTable_Cloud.AcceptChanges();

            UI_DATA.ServerConnectsTable_Cloud.RefreshDataTable();
            m_UserDataSQLite.SaveDataTable(UI_DATA.ServerConnectsTable_Cloud, "CloudServerConnList");
        }

        /// <summary>
        /// 子窗口数据传递事件
        /// </summary>
        private void window_ReceiveConnectionValues(object sender, CloudConnectionEventArgs connectionArgs)
        {
            if (connectionArgs.IsNewRow)
            {
                UI_DATA.ServerConnectsTable_Cloud.Rows.Add(connectionArgs.GetRow);

                UI_DATA.ServerConnectsTable_Cloud.RefreshDataTable();
                m_UserDataSQLite.SaveDataTable(UI_DATA.ServerConnectsTable_Cloud, "CloudServerConnList");
            }
            else
            {
                m_UserDataSQLite.UpdateDataTable(UI_DATA.ServerConnectsTable_Cloud, "CloudServerConnList");
            }
        }

        #endregion ----------------------------------------------------------------------------------------------------

        #region [远程服务器 服务器列表功能区]----------------------------------------------------------------------------------------------------


        private void button_CloudServer_AddServer_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.Filter = "EXE - File(*.exe) | *.exe| 所有文件(*.*) | *.*";
            openFile.ShowDialog();
        }


        private void button_CloudServer_GetServer_Click(object sender, RoutedEventArgs e)
        {
            string location = (dataGrid_CloudServer_Connections.SelectedItem as DataRowView)[1].ToString();
            string username = (dataGrid_CloudServer_Connections.SelectedItem as DataRowView)[2].ToString();
            string password = (dataGrid_CloudServer_Connections.SelectedItem as DataRowView)[3].ToString();


            //  int indexConn = dataGrid_CloudServer_Connection.SelectedIndex;

            //  m_ServerConnect[indexConn].GetSshClient.RunCommand("").Execute();

            //textBox_Servers_Tab_Log.Text += m_Current_SshClient.RunCommand("top").Execute();
            // m_ServerConnect[indexConn].GetSshClient.CreateShell()


            //m_ServerConnect[1].SendCommand("top");
        }


        #endregion ----------------------------------------------------------------------------------------------------

        #region [远程服务器 集群存档功能区]----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 远程服务器-存档文件夹选择变化
        /// </summary>
        private void ComboBox_CloudServer_SavesFolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string location = (dataGrid_CloudServer_Connections.SelectedItem as DataRowView)[1].ToString();
            string username = (dataGrid_CloudServer_Connections.SelectedItem as DataRowView)[2].ToString();
            string password = (dataGrid_CloudServer_Connections.SelectedItem as DataRowView)[3].ToString();

            SftpClient client = null;
            foreach (var item in m_ServerConnect)
            {
                if (location == item.Location && username == item.UserName && password == item.Password)
                    client = item.GetSftpClient;
            }
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
            RefreshServersData(m_ClusterInfo_Cloud[index], ref UI_DATA);
            if (m_ClusterInfo_Cloud[index].ClusterServers.Count != 0) dataGrid_ClusterInfo_ServersList.SelectedIndex = 0;
        }

        private void button_CloudServer_StartCluster_Click(object sender, RoutedEventArgs e)
        {
            int indexCloudFile = listBox_CloudServer_ClusterFile.SelectedIndex;

            string location = (dataGrid_CloudServer_Connections.SelectedItem as DataRowView)[1].ToString();
            string userName = (dataGrid_CloudServer_Connections.SelectedItem as DataRowView)[2].ToString();
            string password = (dataGrid_CloudServer_Connections.SelectedItem as DataRowView)[3].ToString();

            string confdir = ComboBox_CloudServer_SavesFolder.SelectedItem.ToString();
            string cluster = listBox_CloudServer_ClusterFile.SelectedItem.ToString();
            string exefile = (dataGrid_CloudServer_ServersPath.SelectedItem as DataRowView)[2].ToString();

            List<string> screenList = ServersManager.GetExistScreens(location, userName, password);

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
                    command = $"screen -S {screenName} {exefile} {parameter}";
                }

                CreatNewScreens(location, userName, password, command);
            }
        }

        #endregion ----------------------------------------------------------------------------------------------------
    }
}
