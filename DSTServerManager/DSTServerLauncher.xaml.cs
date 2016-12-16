﻿using DSTServerManager.DataHelper;
using DSTServerManager.Saves;
using DSTServerManager.Servers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DSTServerManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DSTServerLauncher : Window
    {
        private static string appStartupPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        private static List<ServerConnect> m_ServerConnect = new List<ServerConnect>();
        private static List<ServerProcess> m_ServerProcess = new List<ServerProcess>();
        private static List<ServerScreens> m_ServerScreens = new List<ServerScreens>();

        private List<ClusterInfo> m_ClusterInfo_Local = null;
        private List<ClusterInfo> m_ClusterInfo_Cloud = null;

        private UserInterfaceData UI_DATA = null;

        private SQLiteHelper m_UserDataSQLite = null;

        private CloudConnection m_Win_CloudConnection = null;
        private SteamCommand m_Win_SteamCommand = null;

        private string m_TabItemXaml = string.Empty;

        public DSTServerLauncher()
        {
            InitializeComponent();
            m_TabItemXaml = System.Windows.Markup.XamlWriter.Save(tabItemMain);
            TabControl_ServerLog.SelectionChanged += TabControl_ServerLog_SelectionChanged;

            #region 界面绑定数据初始化

            UI_DATA = new UserInterfaceData(dataGrid_ClusterInfo_ServersList.Columns.Count);
            BindingState();
            
            textBox_BasicInfo_Key.Text = ConfigHelper.GetValue("textBox_BasicInfo_Key");
            dataGrid_LocalServer_ServersPath.FrozenColumnCount = 2;
            dataGrid_CloudServer_ServersPath.FrozenColumnCount = 2;

            BackgroundWorker userdataWorker = new BackgroundWorker();
            userdataWorker.DoWork += UserDataWorker_DoWork;
            userdataWorker.RunWorkerCompleted += UserdataWorker_RunWorkerCompleted;
            userdataWorker.RunWorkerAsync();

            #endregion
        }

        private void UserDataWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            m_UserDataSQLite = new SQLiteHelper();
            GetUserData(m_UserDataSQLite);

            if (SavesManager.GetSavesFolder().Count == 0) SavesManager.CreatSavesFolder();
            UI_DATA.SaveFolders_Local = SavesManager.GetSavesFolder();

            foreach (var item in ServersManager.GetExistProcess())
            {
                textBox_Servers_Tab_Log.Text += item + "\r\n";
            }
        }
        private void UserdataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ComboBox_LocalServer_SavesFolder.SelectedIndex = 0;
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
        /// 集群信息-选择服务器
        /// </summary>
        private void dataGrid_ClusterInfo_ServersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int indexLocalFile = listBox_LocalServer_ClusterFile.SelectedIndex;
            int indexCloudFile = listBox_CloudServer_ClusterFile.SelectedIndex;
            int indexServer = dataGrid_ClusterInfo_ServersList.SelectedIndex;

            if (indexServer != -1 && indexLocalFile != -1)
            {
                dataGrid_ClusterInfo_ServerLevel.ItemsSource = m_ClusterInfo_Local[indexLocalFile].ClusterServers[indexServer].Level.ServerLevelTable.DefaultView;
            }
            if (indexServer != -1 && indexCloudFile != -1)
            {
                dataGrid_ClusterInfo_ServerLevel.ItemsSource = m_ClusterInfo_Cloud[indexCloudFile].ClusterServers[indexServer].Level.ServerLevelTable.DefaultView;
            }
        }

        /// <summary>
        /// 本地服务器-保存集群当前配置
        /// </summary>
        private void button_Cluster_SaveIni_Click(object sender, RoutedEventArgs e)
        {
            int indexLocalServer_ClusterFile = listBox_LocalServer_ClusterFile.SelectedIndex;

            if (indexLocalServer_ClusterFile == -1) return;

            //保存当前选中的集群配置
            ExtendHelper.CopyAllProperties(UI_DATA, m_ClusterInfo_Local[indexLocalServer_ClusterFile].ClusterSetting);
            SavesManager.SetClusterInfo(ComboBox_LocalServer_SavesFolder.SelectedItem?.ToString(), m_ClusterInfo_Local[indexLocalServer_ClusterFile]);
        }

        /// <summary>
        /// 保存服务器当前配置
        /// </summary>
        private void button_Server_SaveIni_Click(object sender, RoutedEventArgs e)
        {
            int indexLocalServer_ClusterFile = listBox_LocalServer_ClusterFile.SelectedIndex;

            string nameSave = ComboBox_LocalServer_SavesFolder.SelectedItem?.ToString();
            string nameCluster = listBox_LocalServer_ClusterFile.SelectedItem?.ToString();


            ExtendHelper.CopyAllProperties(UI_DATA, m_ClusterInfo_Local[indexLocalServer_ClusterFile]);

            for (int i = 0; i < m_ClusterInfo_Local[indexLocalServer_ClusterFile].ClusterServers.Count; i++)
                SavesManager.SetServerInfo(nameSave, nameCluster, m_ClusterInfo_Local[indexLocalServer_ClusterFile].ClusterServers[i]);
        }

        private void textBox_BasicInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConfigHelper.SetValue("textBox_BasicInfo_Key", textBox_BasicInfo_Key.Text);
        }

        private void textBox_Server_Server_Input_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key != Key.Enter) return;

            textBox_Servers_Tab_Log.Text += TabControl_ServerLog.SelectedIndex.ToString();
            textBox_Servers_Tab_Log.Text += sender.ToString() + "\r\n";

            SendCommand(textBox_Server_Server_Input.Text);
        }

        private void dataGrid_Server_Command_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            textBox_Server_Server_Input.Text = ((DataRowView)dataGrid_ServersInfo_CommandLine.SelectedItem)[3].ToString();
            textBox_Server_Server_Input.Focus();
            textBox_Server_Server_Input.CaretIndex = textBox_Server_Server_Input.Text.Length - 1;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }






        //dataGrid和datatable之间数据直接赋值的示例 不应该使用这种方式
        //应该dataGrid绑定一个datatable,然后给datatable的数据进行改变
        // m_ClusterInfo_Local[indexCluster].ClusterServerTable = ((DataView)dataGrid_Cluster_Servers.ItemsSource).Table;
        // dataGrid_LocalServer_ServerList.ItemsSource = ConfigHelper.GetTable().DefaultView;
    }
}
