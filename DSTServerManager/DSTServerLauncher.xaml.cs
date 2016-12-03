using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DSTServerManager.DataHelper;
using DSTServerManager.Saves;
using System.Data.OleDb;
using DSTServerManager.Servers;
using System.Diagnostics;
using Renci.SshNet;
using System.Data;
using System.IO;

namespace DSTServerManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DSTServerLauncher : Window
    {
        string appStartupPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        private List<ClusterInfo> m_ClusterInfo_Local = null;
        private List<ClusterInfo> m_ClusterInfo_Cloud = null;

        private UserInterfaceData m_UI_DATA = null;

        SQLiteHelper m_UserDataSQLite = null;

        private DSTServerCloudSub m_DSTServerCloudSub = null;

        List<ServerProcess> m_ServerProcess = null;
        List<ServerScreens> m_ServerScreens = null;

        public DSTServerLauncher()
        {
            InitializeComponent();

            #region 界面绑定数据初始化
            m_UI_DATA = new UserInterfaceData(dataGrid_Cluster_Servers.Columns.Count);

            BindingState();

            m_UserDataSQLite = new SQLiteHelper();
            GetUserData(m_UserDataSQLite);
            #endregion

            #region 全局变量初始化
            m_ServerProcess = new List<ServerProcess>();
            m_ServerScreens = new List<ServerScreens>();
            #endregion

            List<string> saveFolders_Local = SavesManager.GetSavesFolder();
            //没有要生成一个默认的.....待添加


            foreach (var item in saveFolders_Local) comboBox_SavesFolder_Local.Items.Add(item);
            if (comboBox_SavesFolder_Local.Items.Count != 0) comboBox_SavesFolder_Local.SelectedIndex = 0;
        }

        private void MenuItem_SelectLanguage_Chinese_Click(object sender, RoutedEventArgs e)
        {
            LoadLanguageFile("/Language/zh-cn.xaml");
        }

        private void MenuItem_SelectLanguage_English_Click(object sender, RoutedEventArgs e)
        {
            LoadLanguageFile("/Language/en-us.xaml");
        }

        void LoadLanguageFile(string languagefileName)
        {
            Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
            {
                Source = new Uri(languagefileName, UriKind.RelativeOrAbsolute)
            };
        }

        SftpClient sftpclient = null;
        private void button_Click(object sender, RoutedEventArgs e)
        {
          
        }

        /// <summary>
        /// 开启当前服务器集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Cluster_Start_Click(object sender, RoutedEventArgs e)
        {
            int indexCluster = listBox_Cluster_Local.SelectedIndex;
            int indexServerPath = dataGrid_LocalServer_ServerList.SelectedIndex;
            if (indexCluster != -1 && indexServerPath != -1)
            {
                //保存集群配置
                ExtendHelper.CopyAllProperties(m_UI_DATA, m_ClusterInfo_Local[indexCluster].ClusterSetting);
                SavesManager.SetClusterInfo(comboBox_SavesFolder_Local.SelectedItem?.ToString(), m_ClusterInfo_Local[indexCluster]);

                //服务器和标签页关键信息获取
                string exeName = (dataGrid_LocalServer_ServerList.SelectedItem as DataRowView)[2].ToString();
                if (!File.Exists(exeName)) return;

                //服务器状态列表检查
                for (int i = 0; i < m_ServerProcess.Count; i++)
                {
                    if (m_ServerProcess[i].IsProcessActive == false)
                    {
                        m_ServerProcess[i] = null;
                        m_ServerProcess.Remove(m_ServerProcess[i]);
                    }
                }
                string xaml = System.Windows.Markup.XamlWriter.Save(tabItemMain);

                //获取集群服务器
                foreach (var server in m_ClusterInfo_Local[indexCluster].ClusterServers)
                {
                    StringBuilder cmdBuilder = new StringBuilder(256);
                    //cmdBuilder.Append($" -console");
                    cmdBuilder.Append($" -conf_dir {comboBox_SavesFolder_Local.SelectedItem.ToString()}");
                    cmdBuilder.Append($" -cluster {listBox_Cluster_Local.SelectedItem.ToString()}");
                    cmdBuilder.Append($" -shard {server.Folder}");

                    TabItem newProcessTab = new TabItem();
                    newProcessTab = System.Windows.Markup.XamlReader.Parse(xaml) as TabItem;
                    newProcessTab.Header = server.Folder;
                    tabControl_ServerLog.Items.Add(newProcessTab);

                    ServerProcess process = new ServerProcess(this, tabControl_ServerLog, newProcessTab, false, server.Session);
                    process.StartProcess(exeName, cmdBuilder.ToString());
                    m_ServerProcess.Add(process);
                }
            }
        }

        /// <summary>
        /// 保存集群当前配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Cluster_SaveIni_Click(object sender, RoutedEventArgs e)
        {
            int indexCluster = listBox_Cluster_Local.SelectedIndex;
            if (indexCluster != -1)
            {
                ExtendHelper.CopyAllProperties(m_UI_DATA, m_ClusterInfo_Local[indexCluster].ClusterSetting);
                SavesManager.SetClusterInfo(comboBox_SavesFolder_Local.SelectedItem?.ToString(), m_ClusterInfo_Local[indexCluster]);
            }
        }

        /// <summary>
        /// 保存服务器当前配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Server_SaveIni_Click(object sender, RoutedEventArgs e)
        {
            int indexCluster = listBox_Cluster_Local.SelectedIndex;

            string nameSave = comboBox_SavesFolder_Local.SelectedItem?.ToString();
            string nameCluster = listBox_Cluster_Local.SelectedItem?.ToString();


            ExtendHelper.CopyAllProperties(m_UI_DATA, m_ClusterInfo_Local[indexCluster]);

            for (int i = 0; i < m_ClusterInfo_Local[indexCluster].ClusterServers.Count; i++)
                SavesManager.SetServerInfo(nameSave, nameCluster, m_ClusterInfo_Local[indexCluster].ClusterServers[i]);
        }

        /// <summary>
        /// 选择服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_Save_Cluster_Servers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int indexCluster_Local = listBox_Cluster_Local.SelectedIndex;
            int indexCluster_Cloud = listBox_Cluster_Cloud.SelectedIndex;
            int indexServers = dataGrid_Cluster_Servers.SelectedIndex;

            if (indexServers != -1 && indexCluster_Local != -1)
            {
                dataGrid_Server_Leveled.ItemsSource = m_ClusterInfo_Local[indexCluster_Local].ClusterServers[indexServers].Level.ServerLevelTable.DefaultView;
            }
            if (indexServers != -1 && indexCluster_Cloud != -1)
            {
                dataGrid_Server_Leveled.ItemsSource = m_ClusterInfo_Cloud[indexCluster_Cloud].ClusterServers[indexServers].Level.ServerLevelTable.DefaultView;
            }
        }

        private void textBox_BasicInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConfigHelper.SetValue("textBox_BasicInfo_Key", textBox_BasicInfo_Key.Text);
        }

        private void textBox_Server_Server_Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            textBox_Servers_Tab_Log.Text += tabControl_ServerLog.SelectedIndex.ToString();
            textBox_Servers_Tab_Log.Text += sender.ToString() + "\r\n";

            foreach (var server in m_ServerProcess)
            {
                if (server.ServerTab.Equals(tabControl_ServerLog.SelectedItem))
                {
                    textBox_Servers_Tab_Log.Text += server.ServerSession;
                    server.SendCommand(textBox_Server_Server_Input.Text);
                    //server.SendCommandNormal(textBox_Server_Server_Input.Text);
                    //item.CurrentServerProcess.StandardInput.WriteLine(textBox_Server_Server_Input.Text);
                }
            }
        }

        private void dataGrid_Server_Command_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            textBox_Server_Server_Input.Text = ((DataRowView)dataGrid_Server_Command.SelectedItem)[2].ToString();
            textBox_Server_Server_Input.Focus();
            textBox_Server_Server_Input.CaretIndex = textBox_Server_Server_Input.Text.Length - 1;
        }

        private void button_Cluster_Refresh_Click(object sender, RoutedEventArgs e)
        {
            //获取集群信息
            string saveFolder = comboBox_SavesFolder_Local.SelectedItem?.ToString();
            if (saveFolder == "") return;
            RefreshClusterData(saveFolder, "Cluster", ref listBox_Cluster_Local, null);
            m_ClusterInfo_Local = SavesManager.GetClusterInfo("Cluster", saveFolder);
            if (listBox_Cluster_Local.Items.Count != 0) listBox_Cluster_Local.SelectedIndex = 0;

            int index = listBox_Cluster_Local.SelectedIndex;
            if (index == -1) return;
            RefreshServersData(m_ClusterInfo_Local[index], ref m_UI_DATA);
            if (m_ClusterInfo_Local[index].ClusterServers.Count != 0) dataGrid_Cluster_Servers.SelectedIndex = 0;
        }

        /// <summary>
        /// 本地服务器存档文件夹选择变化回调函数
        /// </summary>
        private void comboBox_SavesFolder_Local_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //获取集群信息
            string saveFolder = comboBox_SavesFolder_Local.SelectedItem?.ToString();
            if (saveFolder == "") return;
            RefreshClusterData(saveFolder, "Cluster", ref listBox_Cluster_Local, null);
            m_ClusterInfo_Local = SavesManager.GetClusterInfo(saveFolder, "Cluster");
            if (listBox_Cluster_Local.Items.Count != 0) listBox_Cluster_Local.SelectedIndex = 0;
        }

        /// <summary>
        /// 远程Linux服务器存档文件夹选择变化回调函数
        /// </summary>
        private void comboBox_SavesFolder_Cloud_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //获取集群信息
            string saveFolder = comboBox_SavesFolder_Cloud.SelectedItem?.ToString();
            if (saveFolder == "") return;
            RefreshClusterData(saveFolder, "Cluster", ref listBox_Cluster_Cloud, sftpclient);
            m_ClusterInfo_Cloud = SavesManager.GetClusterInfo("DoNotStarveTogether", "Cluster", sftpclient);
            if (listBox_Cluster_Cloud.Items.Count != 0) listBox_Cluster_Cloud.SelectedIndex = 0;
        }

        private void listBox_Cluster_Local_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = listBox_Cluster_Local.SelectedIndex;
            if (index == -1) return;
            RefreshServersData(m_ClusterInfo_Local[index], ref m_UI_DATA);
            if (m_ClusterInfo_Local[index].ClusterServers.Count != 0) dataGrid_Cluster_Servers.SelectedIndex = 0;
        }

        private void listBox_Cluster_Cloud_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = listBox_Cluster_Cloud.SelectedIndex;
            if (index == -1) return;
            RefreshServersData(m_ClusterInfo_Cloud[index], ref m_UI_DATA);
            if (m_ClusterInfo_Cloud[index].ClusterServers.Count != 0) dataGrid_Cluster_Servers.SelectedIndex = 0;
        }

        private void button_LocalServer_AddServer_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.Filter = "EXE - File(*.exe) | *.exe| 所有文件(*.*) | *.*";
            openFile.ShowDialog();
        }

        private void button_CloudServer_AddServer_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.Filter = "EXE - File(*.exe) | *.exe| 所有文件(*.*) | *.*";
            openFile.ShowDialog();
        }

        /// <summary>
        /// 增加远程服务器链接
        /// </summary>
        private void button_CloudServer_AddConn_Click(object sender, RoutedEventArgs e)
        {
            DataRow currentRow = m_UI_DATA.ServerConnectsTable_Cloud.NewRow();
            int newIndex = m_UI_DATA.ServerConnectsTable_Cloud.Rows.Count + 1;

            if (m_DSTServerCloudSub == null) m_DSTServerCloudSub = new DSTServerCloudSub(currentRow, true, newIndex);

            m_DSTServerCloudSub.PassValuesEvent += new DSTServerCloudSub.PassValuesHandler(window_ReceiveConnectionValues);
            m_DSTServerCloudSub.Show();

            m_DSTServerCloudSub.Closed += (object sender2, EventArgs e2) => { m_DSTServerCloudSub = null; };
        }

        /// <summary>
        /// 编辑当前选中的远程服务器链接
        /// </summary>
        private void button_CloudServer_EditConn_Click(object sender, RoutedEventArgs e)
        {
            int indexConn = dataGrid_CloudServer_Connection.SelectedIndex;
            if (indexConn == -1) return;

            DataRow currentRow = m_UI_DATA.ServerConnectsTable_Cloud.Rows[indexConn];
            if (m_DSTServerCloudSub == null) m_DSTServerCloudSub = new DSTServerCloudSub(currentRow, false, 0);

            m_DSTServerCloudSub.PassValuesEvent += new DSTServerCloudSub.PassValuesHandler(window_ReceiveConnectionValues);
            m_DSTServerCloudSub.Show();

            m_DSTServerCloudSub.Closed += (object sender2, EventArgs e2) => { m_DSTServerCloudSub = null; };
        }

        /// <summary>
        /// 删除当前选中的远程服务器链接
        /// </summary>
        private void button_CloudServer_DeleteConn_Click(object sender, RoutedEventArgs e)
        {
            int indexConn = dataGrid_CloudServer_Connection.SelectedIndex;
            if (indexConn == -1) return;

            m_UI_DATA.ServerConnectsTable_Cloud.Rows[indexConn].Delete();
            m_UI_DATA.ServerConnectsTable_Cloud.AcceptChanges();

            string exception = string.Empty;

            m_UI_DATA.ServerConnectsTable_Cloud.RefreshDataTable(out exception);
            m_UserDataSQLite.SaveDataTable(m_UI_DATA.ServerConnectsTable_Cloud, "CloudServerConnList", out exception);
        }

        /// <summary>
        /// 子窗口数据传递事件
        /// </summary>
        private void window_ReceiveConnectionValues(object sender, PassValuesEventArgs passValue)
        {
            string exception = string.Empty;
            if (passValue.IsNewRow)
            {
                m_UI_DATA.ServerConnectsTable_Cloud.Rows.Add(passValue.GetRow);

                m_UI_DATA.ServerConnectsTable_Cloud.RefreshDataTable(out exception);
                m_UserDataSQLite.SaveDataTable(m_UI_DATA.ServerConnectsTable_Cloud, "CloudServerConnList", out exception);
            }
            else
            {
                m_UserDataSQLite.UpdateDataTable(m_UI_DATA.ServerConnectsTable_Cloud, "CloudServerConnList", out exception);
            }
        }

        private void dataGrid_CloudServer_Connection_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            List<string> saveFolders_Cloud = SavesManager.GetSavesFolder(sftpclient);
            foreach (var item in saveFolders_Cloud)
                comboBox_SavesFolder_Cloud.Items.Add(item);
            comboBox_SavesFolder_Cloud.SelectedIndex = 0;
        }


        //dataGrid和datatable之间数据直接赋值的示例 不应该使用这种方式
        //应该dataGrid绑定一个datatable,然后给datatable的数据进行改变
        // m_ClusterInfo_Local[indexCluster].ClusterServerTable = ((DataView)dataGrid_Cluster_Servers.ItemsSource).Table;
        // dataGrid_LocalServer_ServerList.ItemsSource = ConfigHelper.GetTable().DefaultView;
    }
}
