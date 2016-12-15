using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DSTServerManager.Saves;
using DSTServerManager.Servers;
using System.Diagnostics;
using System.Data.OleDb;
using System.Globalization;
using System.Data;
using Renci.SshNet;
using DSTServerManager.DataHelper;
using System.ComponentModel;
using System.IO;

namespace DSTServerManager
{
    public partial class DSTServerLauncher : Window
    {
        #region [远程Connect]----------------------------------------------------------------------------------------------------

        internal void CreatNewConnect(string ip, string userName, string password)
        {
            RefreshList();
            TabItem connectTab = System.Windows.Markup.XamlReader.Parse(m_TabItemXaml) as TabItem;
            connectTab.MouseDoubleClick += ConnectTab_MouseDoubleClick;
            tabControl_ServerLog.Items.Add(connectTab);

            ServerConnect serverConnect = new ServerConnect(ip, userName, password);
            serverConnect.CreatTabWindow(tabControl_ServerLog, connectTab);

            //在后台线程开始执行
            BackgroundWorker connectWorker = new BackgroundWorker();
            connectWorker.DoWork += ConnectWorker_DoWork;
            connectWorker.RunWorkerCompleted += ConnectWorker_RunWorkerCompleted;
            connectWorker.RunWorkerAsync(new object[] { serverConnect });
        }

        private void ConnectTab_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (var connect in m_ServerConnect)
            {
                if (!connect.ServerTab.Equals(sender as TabItem)) continue;

                textBox_Servers_Tab_Log.Text = sender.ToString();
                tabControl_ServerLog.Items.Remove(sender as TabItem);
            }
        }

        private void ConnectWorker_DoWork(object sender, DoWorkEventArgs passValue)
        {
            object[] argument = (object[])passValue.Argument;

            ServerConnect serverConnect = argument[0] as ServerConnect;
            SftpClient client = serverConnect.GetSftpClient;

            serverConnect.StartConnect();
            m_ServerConnect.Add(serverConnect);

            if (SavesManager.GetSavesFolder(client).Count == 0) SavesManager.CreatSavesFolder(client);
            UI_DATA.SaveFolders_Cloud = SavesManager.GetSavesFolder(client);

            foreach (var item in ServersManager.GetExistScreens(serverConnect))
            {
                textBox_Servers_Tab_Log.Text += item + "\r\n";
            }
        }
        private void ConnectWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs passValue)
        {
            comboBox_SavesFolder_Cloud.SelectedIndex = 0;

            int indexConn = dataGrid_CloudServer_Connections.SelectedIndex;
            //控制DataGrid的颜色
            DataGridRow dataRow = (DataGridRow)dataGrid_CloudServer_Connections.ItemContainerGenerator.ContainerFromIndex(indexConn);
            if (m_ServerConnect[indexConn].AllConnected) dataRow.Background = new SolidColorBrush(Colors.LightGreen);
            else dataRow.Background = new SolidColorBrush(Colors.OrangeRed);
        }

        #endregion ----------------------------------------------------------------------------------------------------

        #region [本地Process]----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 创建本地Process
        /// </summary>
        /// <param name="serverExe"></param>
        /// <param name="parameter"></param>
        /// <param name="session"></param>
        internal void CreatNewProcess(string serverExe, string parameter, bool isShell, string session)
        {
            if (!File.Exists(serverExe)) return;

            RefreshList();
            TabItem processTab = System.Windows.Markup.XamlReader.Parse(m_TabItemXaml) as TabItem;
            processTab.MouseDoubleClick += ProcessTab_MouseDoubleClick;
            tabControl_ServerLog.Items.Add(processTab);

            ServerProcess process = new ServerProcess(isShell, session);
            process.CreatTabWindow(tabControl_ServerLog, processTab);

            //在后台线程开始执行
            BackgroundWorker processWorker = new BackgroundWorker();
            processWorker.DoWork += ProcessWorker_DoWork;
            processWorker.RunWorkerCompleted += ProcessWorker_RunWorkerCompleted;
            processWorker.RunWorkerAsync(new object[] { process, serverExe, parameter });
        }

        private void ProcessTab_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (var servers in m_ServerProcess)
            {
                if (!servers.ServerTab.Equals(sender as TabItem)) continue;

                textBox_Servers_Tab_Log.Text = sender.ToString();
                servers.SendCommand("c_shutdown()");
            }
        }

        private void ProcessWorker_DoWork(object sender, DoWorkEventArgs passValue)
        {
            object[] argument = (object[])passValue.Argument;

            ServerProcess process = argument[0] as ServerProcess;
            string serverExe = argument[1] as string;
            string parameter = argument[2] as string;

            process.StartProcess(serverExe, parameter);
            m_ServerProcess.Add(process);
        }

        private void ProcessWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion ----------------------------------------------------------------------------------------------------

        #region [远程Screens]----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 创建远程Screens
        /// </summary>
        /// <param name="serverExe"></param>
        /// <param name="parameter"></param>
        /// <param name="session"></param>
        internal void CreatNewScreens(string ip, string userName, string password)
        {
            RefreshList();
            TabItem connectTab = System.Windows.Markup.XamlReader.Parse(m_TabItemXaml) as TabItem;
            tabControl_ServerLog.Items.Add(connectTab);

            ServerScreens serverConnect = new ServerScreens(ip, userName, password);
            serverConnect.CreatTabWindow(tabControl_ServerLog, connectTab);

            //在后台线程开始执行
            BackgroundWorker connectWorker = new BackgroundWorker();
            connectWorker.DoWork += ConnectWorker_DoWork;
            connectWorker.RunWorkerCompleted += ConnectWorker_RunWorkerCompleted;
            connectWorker.RunWorkerAsync(new object[] { serverConnect });
        }

        private void ScreensWorker_DoWork(object sender, DoWorkEventArgs passValue)
        {
            object[] argument = (object[])passValue.Argument;

            ServerConnect serverConnect = argument[0] as ServerConnect;
            SftpClient client = serverConnect.GetSftpClient;

            serverConnect.StartConnect();
            //serverConnect.SendCommand
        }

        private void ScreensWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 发送控制命令
        /// </summary>
        /// <param name="command"></param>
        internal void SendCommand(string command)
        {
            foreach (var servers in m_ServerProcess)
            {
                if (!servers.ServerTab.Equals(tabControl_ServerLog.SelectedItem)) continue;

                servers.SendCommand(command);
            }

            foreach (var connect in m_ServerConnect)
            {
                if (!connect.AllConnected) continue;
                if (!connect.ServerTab.Equals(tabControl_ServerLog.SelectedItem)) continue;

                connect.SendCommand(command);
            }
        }

        /// <summary>
        /// 状态列表检查
        /// </summary>
        internal void RefreshList()
        {
            for (int i = 0; i < m_ServerProcess.Count; i++)
            {
                if (m_ServerProcess[i].IsProcessActive == true) continue;

                m_ServerProcess[i] = null;
                m_ServerProcess.Remove(m_ServerProcess[i]);
            }
        }
    }
}
