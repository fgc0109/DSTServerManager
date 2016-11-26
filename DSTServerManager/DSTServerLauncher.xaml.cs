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
using DSTServerManager.Servers;
using System.Diagnostics;
using System.Data;

namespace DSTServerManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DSTServerLauncher : Window
    {
        private List<string> m_ClusterFolder = null;
        private List<ClusterInfo> m_ClusterInfo = null;
        private List<LocalServerInfo> m_LocalServerInfo = null;
        private List<CloudServerInfo> m_CloudServerInfo = null;

        private ClusterIni m_UserInterfaceIniData = null;
        private ClusterInfo m_UserInterfaceServerData = null;

        List<ServerProcess> m_ServerProcess = null;
        DataTable m_ServerListTable = null;

        public DSTServerLauncher()
        {
            InitializeComponent();

            m_UserInterfaceIniData = new ClusterIni();
            m_UserInterfaceServerData = new ClusterInfo(dataGrid_Cluster_Servers.Columns.Count);
            m_CloudServerInfo = new List<CloudServerInfo>();
            m_LocalServerInfo = new List<LocalServerInfo>();
            m_ServerProcess = new List<ServerProcess>();
            BindingState();

            //获取用户配置文件数据
            textBox_BasicInfo_Key.Text = ConfigHelper.GetValue("textBox_BasicInfo_Key");
            textBox_BasicInfo_Path_Steam.Text = ConfigHelper.GetValue("textBox_BasicInfo_Path_Steam");
            textBox_BasicInfo_Path_TGPGM.Text = ConfigHelper.GetValue("textBox_BasicInfo_Path_TGPGM");
            textBox_BasicInfo_Path_Linux.Text = ConfigHelper.GetValue("textBox_BasicInfo_Path_TGPGM");


            textBox_BasicInfo_Path_Steam.Text = @"E:\SteamLibrary\steamapps\common\Don't Starve Together Dedicated Server\bin\dontstarve_dedicated_server_nullrenderer.exe";

            //获取游戏数据表
            dataGrid_Server_Command.ItemsSource = ServerConsole.GetServerConsoleCommandData().DefaultView;

            List<string> saveFolders = SavesManager.GetSavesFolder();
            foreach (var item in saveFolders)
                comboBox_SavesFolder.Items.Add(item);
            comboBox_SavesFolder.SelectedIndex = 0;
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
        private void button_Click(object sender, RoutedEventArgs e)
        {
            ServerDefault newdata = new ServerDefault();
        }

        private void comboBox_SavesFolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshClusterData("Cluster");
        }

        private void listBox_Cluster_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshServersData();
        }


        /// <summary>
        /// 开启当前服务器集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Cluster_Start_Click(object sender, RoutedEventArgs e)
        {
            int index = listBox_Cluster.SelectedIndex;
            if (index != -1)
            {
                //textBox_Servers_Tab_Log.Text = listBox_Cluster.SelectedItem.ToString();

                //服务器和标签页关键信息获取
                string exeName = string.Empty;
                if ((bool)radioButton_BasicInfo_Path_Steam.IsChecked)
                    exeName = textBox_BasicInfo_Path_Steam.Text;
                if ((bool)radioButton_BasicInfo_Path_TGPGM.IsChecked)
                    exeName = textBox_BasicInfo_Path_TGPGM.Text;
                if ((bool)radioButton_BasicInfo_Path_Linux.IsChecked)
                    exeName = textBox_BasicInfo_Path_Linux.Text;

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
                foreach (var server in m_ClusterInfo[index].Servers)
                {
                    StringBuilder cmdBuilder = new StringBuilder(256);
                    //cmdBuilder.Append($" -console");
                    cmdBuilder.Append($" -conf_dir {comboBox_SavesFolder.SelectedItem.ToString()}");
                    cmdBuilder.Append($" -cluster {listBox_Cluster.SelectedItem.ToString()}");
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

        private void button_Test_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < m_LocalServerInfo.Count; i++)
            {
                m_LocalServerInfo[i] = null;
            }
        }

        /// <summary>
        /// 保存集群当前配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Cluster_SaveIni_Click(object sender, RoutedEventArgs e)
        {
            int indexCluster = listBox_Cluster.SelectedIndex;
            if (indexCluster != -1)
            {
                CopyHelper.CopyAllProperties(m_UserInterfaceIniData, m_ClusterInfo[indexCluster].Setting);
                SavesManager.SetClusterInfo(comboBox_SavesFolder.SelectedItem?.ToString(), m_ClusterInfo[indexCluster]);
            }
        }

        /// <summary>
        /// 保存服务器当前配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Server_SaveIni_Click(object sender, RoutedEventArgs e)
        {
            int indexCluster = listBox_Cluster.SelectedIndex;

            string nameSave = comboBox_SavesFolder.SelectedItem?.ToString();
            string nameCluster = listBox_Cluster.SelectedItem?.ToString();

            m_ClusterInfo[indexCluster].ClusterServerTable = ((DataView)dataGrid_Cluster_Servers.ItemsSource).Table;
            for (int i = 0; i < m_ClusterInfo[indexCluster].Servers.Count; i++)
                SavesManager.SetServerInfo(nameSave, nameCluster, m_ClusterInfo[indexCluster].Servers[i]);
        }

        /// <summary>
        /// 选择服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_Save_Cluster_Servers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int indexCluster = listBox_Cluster.SelectedIndex;
            int indexServers = dataGrid_Cluster_Servers.SelectedIndex;

            if (indexServers != -1)
            {
                dataGrid_ServerLevel.ItemsSource = m_ClusterInfo[indexCluster].Servers[indexServers].Level.ServerLevelTable.DefaultView;
            }
        }

        private void textBox_BasicInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConfigHelper.SetValue("textBox_BasicInfo_Key", textBox_BasicInfo_Key.Text);

            ConfigHelper.SetValue("textBox_BasicInfo_Path_Steam", textBox_BasicInfo_Path_Steam.Text);
            ConfigHelper.SetValue("textBox_BasicInfo_Path_TGPGM", textBox_BasicInfo_Path_TGPGM.Text);
            ConfigHelper.SetValue("textBox_BasicInfo_Path_Linux", textBox_BasicInfo_Path_Linux.Text);
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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            textBox_Server_Server_Input.Text = ((DataRowView)dataGrid_Server_Command.SelectedItem)[2].ToString();
            textBox_Server_Server_Input.Focus();
            textBox_Server_Server_Input.CaretIndex = textBox_Server_Server_Input.Text.Length - 1;
        }

        private void radioButton_BasicInfo_Path_Click(object sender, RoutedEventArgs e)
        {
            ConfigHelper.SetValue(radioButton_BasicInfo_Path_Steam.Name.ToString(), radioButton_BasicInfo_Path_Steam.IsChecked?.ToString());
            ConfigHelper.SetValue(radioButton_BasicInfo_Path_TGPGM.Name.ToString(), radioButton_BasicInfo_Path_TGPGM.IsChecked?.ToString());
            ConfigHelper.SetValue(radioButton_BasicInfo_Path_Linux.Name.ToString(), radioButton_BasicInfo_Path_Linux.IsChecked?.ToString());

            //textBox_Servers_Tab_Log.Text = ConfigHelper.GetBoolValue(radioButton_BasicInfo_Path_Steam.Name.ToString()).ToString();
            //textBox_Servers_Tab_Log.Text += ConfigHelper.GetBoolValue(radioButton_BasicInfo_Path_TGPGM.Name.ToString()).ToString();
            //textBox_Servers_Tab_Log.Text += ConfigHelper.GetBoolValue(radioButton_BasicInfo_Path_Linux.Name.ToString()).ToString();
        }

        private void button_Cluster_Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshClusterData("Cluster");
            RefreshServersData();
        }
    }
}
