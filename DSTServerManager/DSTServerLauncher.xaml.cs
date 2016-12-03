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

        private UserInterfaceData UI_DATA = null;

        private SQLiteHelper m_UserDataSQLite = null;

        private SubWindow_CloudConnection m_DSTServerCloudSub = null;

        private List<ServerConnect> m_ServerConnect = null;
        private List<ServerProcess> m_ServerProcess = null;
        private List<ServerScreens> m_ServerScreens = null;

        SftpClient m_Current_SftpClient = null;

        public DSTServerLauncher()
        {
            InitializeComponent();

            #region 界面绑定数据初始化
            UI_DATA = new UserInterfaceData(dataGrid_ClusterInfo_ServersList.Columns.Count);
            BindingState();

            m_UserDataSQLite = new SQLiteHelper();
            GetUserData(m_UserDataSQLite);
            #endregion

            #region 全局变量初始化
            m_ServerConnect = new List<ServerConnect>();
            m_ServerProcess = new List<ServerProcess>();
            m_ServerScreens = new List<ServerScreens>();
            #endregion

            application_LocalServer_Init();
            application_CloudServer_Init();
        }

        #region $$$ 菜单功能区

        private void MenuItem_SelectLanguage_Chinese_Click(object sender, RoutedEventArgs e)
        {
            LoadLanguageFile("/Language/zh-cn.xaml");
        }

        private void MenuItem_SelectLanguage_English_Click(object sender, RoutedEventArgs e)
        {
            LoadLanguageFile("/Language/en-us.xaml");
        }

        /// <summary>
        /// 加载语言文件
        /// </summary>
        /// <param name="languagefileName"></param>
        void LoadLanguageFile(string languagefileName)
        {
            Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
            {
                Source = new Uri(languagefileName, UriKind.RelativeOrAbsolute)
            };
        }

        #endregion

        /// <summary>
        /// 远程连接列表初始化
        /// </summary>
        private void application_CloudServer_Init()
        {
            if (dataGrid_CloudServer_Connection.Items.Count == 0) return;

            foreach (DataRowView item in dataGrid_CloudServer_Connection.Items)
            {
                ServerConnect serverConnect = new ServerConnect(item[1].ToString(), 22, item[2].ToString(), item[3].ToString());
                m_ServerConnect.Add(serverConnect);
            }
        }

        /// <summary>
        /// 本地服务器-获取默认存档路径下的存档文件夹列表
        /// </summary>
        private void application_LocalServer_Init()
        {
            if (SavesManager.GetSavesFolder().Count == 0) SavesManager.CreatSavesFolder();

            List<string> saveFolders_Local = SavesManager.GetSavesFolder();
            foreach (var item in saveFolders_Local) comboBox_SavesFolder_Local.Items.Add(item);
            comboBox_SavesFolder_Local.SelectedIndex = 0;
        }

        /// <summary>
        /// 远程服务器-获取默认存档路径下的存档文件夹列表
        /// </summary>
        private void dataGrid_CloudServer_Connection_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int indexCloudServer_Connection = dataGrid_CloudServer_Connection.SelectedIndex;
            DataGridRow dataRow = (DataGridRow)this.dataGrid_CloudServer_Connection.ItemContainerGenerator.ContainerFromIndex(indexCloudServer_Connection);


            m_Current_SftpClient = m_ServerConnect[indexCloudServer_Connection].GetSftpClient;
            if (m_Current_SftpClient.IsConnected == false)
                try
                {
                    m_Current_SftpClient.Connect();
                    dataRow.Background = new SolidColorBrush(Colors.LightGreen);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    dataRow.Background = new SolidColorBrush(Colors.OrangeRed);
                    return;
                }
                finally
                {
                    dataGrid_CloudServer_Connection.SelectedIndex = -1;
                }
            if (SavesManager.GetSavesFolder(m_Current_SftpClient).Count == 0) SavesManager.CreatSavesFolder(m_Current_SftpClient);

            List<string> saveFolders_Cloud = SavesManager.GetSavesFolder(m_Current_SftpClient);
            foreach (var item in saveFolders_Cloud) comboBox_SavesFolder_Cloud.Items.Add(item);
            comboBox_SavesFolder_Cloud.SelectedIndex = 0;
        }

        /// <summary>
        /// 本地服务器-存档文件夹选择变化
        /// </summary>
        private void comboBox_SavesFolder_Local_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //获取集群信息
            string saveFolder = comboBox_SavesFolder_Local.SelectedItem?.ToString();
            if (saveFolder == string.Empty) return;
            RefreshClusterData(saveFolder, "Cluster", ref listBox_LocalServer_ClusterFile, null);
            m_ClusterInfo_Local = SavesManager.GetClusterInfo(saveFolder, "Cluster");
            if (listBox_LocalServer_ClusterFile.Items.Count != 0) listBox_LocalServer_ClusterFile.SelectedIndex = 0;
        }

        /// <summary>
        /// 远程服务器-存档文件夹选择变化
        /// </summary>
        private void comboBox_SavesFolder_Cloud_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //获取集群信息
            string saveFolder = comboBox_SavesFolder_Cloud.SelectedItem?.ToString();
            if (saveFolder == string.Empty) return;
            RefreshClusterData(saveFolder, "Cluster", ref listBox_CloudServer_ClusterFile, m_Current_SftpClient);
            m_ClusterInfo_Cloud = SavesManager.GetClusterInfo(saveFolder, "Cluster", m_Current_SftpClient);
            if (listBox_CloudServer_ClusterFile.Items.Count != 0) listBox_CloudServer_ClusterFile.SelectedIndex = 0;
        }

        /// <summary>
        /// 本地服务器-集群存档文件夹选择变化
        /// </summary>
        private void listBox_LocalServer_ClusterFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = listBox_LocalServer_ClusterFile.SelectedIndex;
            if (index == -1) return;
            RefreshServersData(m_ClusterInfo_Local[index], ref UI_DATA);
            if (m_ClusterInfo_Local[index].ClusterServers.Count != 0) dataGrid_ClusterInfo_ServersList.SelectedIndex = 0;
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

        /// <summary>
        /// 集群信息-选择服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_ClusterInfo_ServersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int indexLocalServer_ClusterFile = listBox_LocalServer_ClusterFile.SelectedIndex;
            int indexCloudServer_ClusterFile = listBox_CloudServer_ClusterFile.SelectedIndex;
            int indexClusterInfo_ServersList = dataGrid_ClusterInfo_ServersList.SelectedIndex;

            if (indexClusterInfo_ServersList != -1 && indexLocalServer_ClusterFile != -1)
            {
                dataGrid_ClusterInfo_ServerLevel.ItemsSource = m_ClusterInfo_Local[indexLocalServer_ClusterFile].ClusterServers[indexClusterInfo_ServersList].Level.ServerLevelTable.DefaultView;
            }
            if (indexClusterInfo_ServersList != -1 && indexCloudServer_ClusterFile != -1)
            {
                dataGrid_ClusterInfo_ServerLevel.ItemsSource = m_ClusterInfo_Cloud[indexCloudServer_ClusterFile].ClusterServers[indexClusterInfo_ServersList].Level.ServerLevelTable.DefaultView;
            }
        }

        /// <summary>
        /// 本地服务器-开启当前服务器集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Cluster_Start_Local_Click(object sender, RoutedEventArgs e)
        {
            int indexLocalServer_ClusterFile = listBox_LocalServer_ClusterFile.SelectedIndex;
            int indexLocalServer_ServersPath = dataGrid_LocalServer_ServersPath.SelectedIndex;

            if (indexLocalServer_ClusterFile == -1 || indexLocalServer_ServersPath == -1) return;

            //保存当前选中的集群配置
            ExtendHelper.CopyAllProperties(UI_DATA, m_ClusterInfo_Local[indexLocalServer_ClusterFile].ClusterSetting);
            SavesManager.SetClusterInfo(comboBox_SavesFolder_Local.SelectedItem?.ToString(), m_ClusterInfo_Local[indexLocalServer_ClusterFile]);

            //服务器程序文件路径获取
            string exeName = (dataGrid_LocalServer_ServersPath.SelectedItem as DataRowView)[2].ToString();
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
            foreach (var server in m_ClusterInfo_Local[indexLocalServer_ClusterFile].ClusterServers)
            {
                StringBuilder cmdBuilder = new StringBuilder(256);
                cmdBuilder.Append($" -conf_dir {comboBox_SavesFolder_Local.SelectedItem.ToString()}");
                cmdBuilder.Append($" -cluster {listBox_LocalServer_ClusterFile.SelectedItem.ToString()}");
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

        /// <summary>
        /// 本地服务器-保存集群当前配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Cluster_SaveIni_Click(object sender, RoutedEventArgs e)
        {
            int indexLocalServer_ClusterFile = listBox_LocalServer_ClusterFile.SelectedIndex;

            if (indexLocalServer_ClusterFile == -1) return;

            //保存当前选中的集群配置
            ExtendHelper.CopyAllProperties(UI_DATA, m_ClusterInfo_Local[indexLocalServer_ClusterFile].ClusterSetting);
            SavesManager.SetClusterInfo(comboBox_SavesFolder_Local.SelectedItem?.ToString(), m_ClusterInfo_Local[indexLocalServer_ClusterFile]);
        }

        /// <summary>
        /// 保存服务器当前配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Server_SaveIni_Click(object sender, RoutedEventArgs e)
        {
            int indexLocalServer_ClusterFile = listBox_LocalServer_ClusterFile.SelectedIndex;

            string nameSave = comboBox_SavesFolder_Local.SelectedItem?.ToString();
            string nameCluster = listBox_LocalServer_ClusterFile.SelectedItem?.ToString();


            ExtendHelper.CopyAllProperties(UI_DATA, m_ClusterInfo_Local[indexLocalServer_ClusterFile]);

            for (int i = 0; i < m_ClusterInfo_Local[indexLocalServer_ClusterFile].ClusterServers.Count; i++)
                SavesManager.SetServerInfo(nameSave, nameCluster, m_ClusterInfo_Local[indexLocalServer_ClusterFile].ClusterServers[i]);
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
            RefreshClusterData(saveFolder, "Cluster", ref listBox_LocalServer_ClusterFile, null);
            m_ClusterInfo_Local = SavesManager.GetClusterInfo("Cluster", saveFolder);
            if (listBox_LocalServer_ClusterFile.Items.Count != 0) listBox_LocalServer_ClusterFile.SelectedIndex = 0;

            int index = listBox_LocalServer_ClusterFile.SelectedIndex;
            if (index == -1) return;
            RefreshServersData(m_ClusterInfo_Local[index], ref UI_DATA);
            if (m_ClusterInfo_Local[index].ClusterServers.Count != 0) dataGrid_ClusterInfo_ServersList.SelectedIndex = 0;
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
            DataRow currentRow = UI_DATA.ServerConnectsTable_Cloud.NewRow();
            int newIndex = UI_DATA.ServerConnectsTable_Cloud.Rows.Count + 1;

            if (m_DSTServerCloudSub == null) m_DSTServerCloudSub = new SubWindow_CloudConnection(currentRow, true, newIndex);

            m_DSTServerCloudSub.PassValuesEvent += new SubWindow_CloudConnection.PassValuesHandler(window_ReceiveConnectionValues);
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

            DataRow currentRow = UI_DATA.ServerConnectsTable_Cloud.Rows[indexConn];
            if (m_DSTServerCloudSub == null) m_DSTServerCloudSub = new SubWindow_CloudConnection(currentRow, false, 0);

            m_DSTServerCloudSub.PassValuesEvent += new SubWindow_CloudConnection.PassValuesHandler(window_ReceiveConnectionValues);
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

            UI_DATA.ServerConnectsTable_Cloud.Rows[indexConn].Delete();
            UI_DATA.ServerConnectsTable_Cloud.AcceptChanges();

            string exception = string.Empty;

            UI_DATA.ServerConnectsTable_Cloud.RefreshDataTable(out exception);
            m_UserDataSQLite.SaveDataTable(UI_DATA.ServerConnectsTable_Cloud, "CloudServerConnList", out exception);
        }

        /// <summary>
        /// 子窗口数据传递事件
        /// </summary>
        private void window_ReceiveConnectionValues(object sender, PassValuesEventArgs passValue)
        {
            string exception = string.Empty;
            if (passValue.IsNewRow)
            {
                UI_DATA.ServerConnectsTable_Cloud.Rows.Add(passValue.GetRow);

                UI_DATA.ServerConnectsTable_Cloud.RefreshDataTable(out exception);
                m_UserDataSQLite.SaveDataTable(UI_DATA.ServerConnectsTable_Cloud, "CloudServerConnList", out exception);
            }
            else
            {
                m_UserDataSQLite.UpdateDataTable(UI_DATA.ServerConnectsTable_Cloud, "CloudServerConnList", out exception);
            }
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {

        }


        private void button_CloudServer_GetServer_Click(object sender, RoutedEventArgs e)
        {

        }


        //dataGrid和datatable之间数据直接赋值的示例 不应该使用这种方式
        //应该dataGrid绑定一个datatable,然后给datatable的数据进行改变
        // m_ClusterInfo_Local[indexCluster].ClusterServerTable = ((DataView)dataGrid_Cluster_Servers.ItemsSource).Table;
        // dataGrid_LocalServer_ServerList.ItemsSource = ConfigHelper.GetTable().DefaultView;
    }
}
