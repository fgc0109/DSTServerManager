using DSTServerManager.DataHelper;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DSTServerManager
{
    public partial class DSTServerLauncher : Window
    {
        /// <summary>
        /// 绑定界面数据
        /// </summary>
        private void BindingState()
        {
            #region 集群配置文件相关内容绑定

            textBox_Cluster_Gameplay_Mode.SetBinding(TextBox.TextProperty, new Binding(nameof(UI.Gameplay_Mode)) { Source = UI });
            textBox_Cluster_Gameplay_Player.SetBinding(TextBox.TextProperty, new Binding(nameof(UI.Gameplay_Player)) { Source = UI });
            comboBox_Cluster_Gameplay_PVP.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding(nameof(UI.Gameplay_PVP), UI));
            comboBox_Cluster_Gameplay_Pause.SetBinding(ComboBox.SelectedIndexProperty, new Binding(nameof(UI.Gameplay_Pause)) { Source = UI });
            comboBox_Cluster_Gameplay_Vote.SetBinding(ComboBox.SelectedIndexProperty, new Binding(nameof(UI.Gameplay_Vote)) { Source = UI });

            textBox_Cluster_Network_Name.SetBinding(TextBox.TextProperty, new Binding(nameof(UI.Network_Name)) { Source = UI });
            textBox_Cluster_Network_Pass.SetBinding(TextBox.TextProperty, new Binding(nameof(UI.Network_Pass)) { Source = UI });
            textBox_Cluster_Network_Intention.SetBinding(TextBox.TextProperty, new Binding(nameof(UI.Network_Intention)) { Source = UI });
            textBox_Cluster_Network_TickRate.SetBinding(TextBox.TextProperty, new Binding(nameof(UI.Network_TickRate)) { Source = UI });
            textBox_Cluster_Network_Timeout.SetBinding(TextBox.TextProperty, new Binding(nameof(UI.Network_Timeout)) { Source = UI });
            textBox_Cluster_Network_Disc.SetBinding(TextBox.TextProperty, new Binding(nameof(UI.Network_Disc)) { Source = UI });

            comboBox_Cluster_Network_Offline.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding(nameof(UI.Network_Offline), UI));
            comboBox_Cluster_Network_LanOnly.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding(nameof(UI.Network_LanOnly), UI));

            comboBox_Cluster_Misc_Console.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding(nameof(UI.Misc_Console), UI));
            comboBox_Cluster_Misc_Mods.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding(nameof(UI.Misc_Mods), UI));

            comboBox_Cluster_Shard_Enabled.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding(nameof(UI.Shard_Enabled), UI));
            textBox_Cluster_Shard_BindIP.SetBinding(TextBox.TextProperty, new Binding(nameof(UI.Shard_BindIP)) { Source = UI });
            textBox_Cluster_Shard_MasterIP.SetBinding(TextBox.TextProperty, new Binding(nameof(UI.Shard_MasterIP)) { Source = UI });
            textBox_Cluster_Shard_MasterPort.SetBinding(TextBox.TextProperty, new Binding(nameof(UI.Shard_MasterPort)) { Source = UI });
            textBox_Cluster_Shard_MasterKey.SetBinding(TextBox.TextProperty, new Binding(nameof(UI.Shard_MasterKey)) { Source = UI });

            #endregion

            //服务器文件路径列表
            dataGrid_CloudServer_ServersPath.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.ServerCloud)) { Source = UI });
            dataGrid_LocalServer_ServersPath.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.ServerLocal)) { Source = UI });

            //远程服务器连接列表
            dataGrid_CloudServer_Connections.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.Connections)) { Source = UI });

            //集群服务器信息列表
            dataGrid_ClusterInfo_ServersList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.ClusterServersTable)) { Source = UI });
            dataGrid_ClusterInfo_ServerLevel.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.ClusterServersLevel)) { Source = UI });

            //服务器控制命令列表
            dataGrid_ServersInfo_CommandLine.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.ClusterCommandLines)) { Source = UI });

            //存档文件夹列表
            ComboBox_LocalServer_SavesFolder.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.SaveFolders_Local)) { Source = UI });
            ComboBox_CloudServer_SavesFolder.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.SaveFolders_Cloud)) { Source = UI });

            //当前选定的远程连接信息
            dataGrid_CloudServer_Connections.SetBinding(DataGrid.SelectedItemProperty, new Binding(nameof(UI.CurrentConn)) { Source = UI });

            //当前服务器模组文件信息
            dataGrid_Modifications_MainInfos.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.Modification)) { Source = UI });
        }

        #region 获取用户配置文件数据

        private DataTable ConnectionsOrigin = new DataTable(nameof(ConnectionsOrigin));
        private DataTable ServerCloudOrigin = new DataTable(nameof(ServerCloudOrigin));
        private DataTable ServerLocalOrigin = new DataTable(nameof(ServerLocalOrigin));
        /// <summary>
        /// 获取用户数据
        /// </summary>
        private void GetUserData(SQLiteHelper userDataSQLite)
        {
            userDataSQLite.OpenSQLite(appStartupPath + @"\DSTServerManager.db");
            CreateDefaultTable(ref userDataSQLite);

            //读取数据库数据
            ConnectionsOrigin = userDataSQLite.ExecuteDataTable(nameof(UI.Connections));
            ServerCloudOrigin = userDataSQLite.ExecuteDataTable(nameof(UI.ServerCloud));
            ServerLocalOrigin = userDataSQLite.ExecuteDataTable(nameof(UI.ServerLocal));

            UI.ClusterCommandLines = userDataSQLite.ExecuteDataTable(nameof(UI.ClusterCommandLines));
            UI.ClusterServersLevel = userDataSQLite.ExecuteDataTable(nameof(UI.ClusterServersLevel));

            //读取并合并外部数据
            if (!File.Exists(appStartupPath + @"\DSTServerManager.xlsx")) return;

            ExcelHelper userDataExcel = new ExcelHelper();
            userDataExcel.OpenExcel(appStartupPath + @"\DSTServerManager.xlsx", ExcelEngines.ACE, ExcelVersion.Office2007);

            ConnectionsOrigin.MergeExcelData(userDataExcel.ExecuteDataTable(nameof(UI.Connections)));
            ServerCloudOrigin.MergeExcelData(userDataExcel.ExecuteDataTable(nameof(UI.ServerCloud)));
            ServerLocalOrigin.MergeExcelData(userDataExcel.ExecuteDataTable(nameof(UI.ServerLocal)));

            UI.ClusterCommandLines.MergeExcelData(userDataExcel.ExecuteDataTable(nameof(UI.ClusterCommandLines)));
            UI.ClusterServersLevel.MergeExcelData(userDataExcel.ExecuteDataTable(nameof(UI.ClusterServersLevel)));

            //更新本地数据库数据
            userDataSQLite.SaveDataTable(ConnectionsOrigin, nameof(UI.Connections));
            userDataSQLite.SaveDataTable(ServerCloudOrigin, nameof(UI.ServerCloud));
            userDataSQLite.SaveDataTable(ServerLocalOrigin, nameof(UI.ServerLocal));

            userDataSQLite.SaveDataTable(UI.ClusterCommandLines, nameof(UI.ClusterCommandLines));
            userDataSQLite.SaveDataTable(UI.ClusterServersLevel, nameof(UI.ClusterServersLevel));
        }

        #endregion

        #region 默认的数据库字段

        /// <summary>
        /// 创建默认的数据表
        /// </summary>
        /// <param name="userDataSQLite"></param>
        /// <param name="exception"></param>
        static private void CreateDefaultTable(ref SQLiteHelper userDataSQLite)
        {
            //创建默认的数据表结构
            string[] parameter = null;
            parameter = new string[3] { "ID integer primary key", "Type text", "Path text" };
            userDataSQLite.CreatDataTable("ServerLocal", parameter);
            userDataSQLite.CreatDataTable("ServerCloud", parameter);

            parameter = new string[5] { "ID integer primary key", "IP text", "UserName text", "Password text", "ServerID text" };
            userDataSQLite.CreatDataTable("Connections", parameter);

            parameter = new string[5] { "ID integer primary key", "Type text", "Explain text", "Command text", "Parameter text" };
            userDataSQLite.CreatDataTable("ClusterCommandLines", parameter);

            parameter = new string[8] { "ID integer primary key", "Name text", "English text", "Chinese text", "WorldType text", "Type text", "Enum text", "Current text" };
            userDataSQLite.CreatDataTable("ClusterServersLevel", parameter);
        }

        #endregion

        private Binding SetBoolBinding(string path, object source)
        {
            Binding boolBinding = new Binding(path);
            boolBinding.Converter = new BoolConvert();
            boolBinding.Source = source;

            return boolBinding;
        }
    }

    /// <summary>
    /// 转换bool变量到comboBox选择序号
    /// 操,原来是测试数据错了  不用转换也能识别
    /// 可以用来转换枚举类型
    /// </summary>
    public class BoolConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) return 1;
            else return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 1) return true;
            else return false;
        }
    }
}
