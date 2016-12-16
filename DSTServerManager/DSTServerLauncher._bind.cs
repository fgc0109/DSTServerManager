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

            textBox_Cluster_Gameplay_Mode.SetBinding(TextBox.TextProperty, new Binding(nameof(UI_DATA.Gameplay_Mode)) { Source = UI_DATA });
            textBox_Cluster_Gameplay_Player.SetBinding(TextBox.TextProperty, new Binding(nameof(UI_DATA.Gameplay_Player)) { Source = UI_DATA });
            comboBox_Cluster_Gameplay_PVP.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding(nameof(UI_DATA.Gameplay_PVP), UI_DATA));
            comboBox_Cluster_Gameplay_Pause.SetBinding(ComboBox.SelectedIndexProperty, new Binding(nameof(UI_DATA.Gameplay_Pause)) { Source = UI_DATA });
            comboBox_Cluster_Gameplay_Vote.SetBinding(ComboBox.SelectedIndexProperty, new Binding(nameof(UI_DATA.Gameplay_Vote)) { Source = UI_DATA });

            textBox_Cluster_Network_Name.SetBinding(TextBox.TextProperty, new Binding(nameof(UI_DATA.Network_Name)) { Source = UI_DATA });
            textBox_Cluster_Network_Pass.SetBinding(TextBox.TextProperty, new Binding(nameof(UI_DATA.Network_Pass)) { Source = UI_DATA });
            textBox_Cluster_Network_Intention.SetBinding(TextBox.TextProperty, new Binding(nameof(UI_DATA.Network_Intention)) { Source = UI_DATA });
            textBox_Cluster_Network_TickRate.SetBinding(TextBox.TextProperty, new Binding(nameof(UI_DATA.Network_TickRate)) { Source = UI_DATA });
            textBox_Cluster_Network_Timeout.SetBinding(TextBox.TextProperty, new Binding(nameof(UI_DATA.Network_Timeout)) { Source = UI_DATA });
            textBox_Cluster_Network_Disc.SetBinding(TextBox.TextProperty, new Binding(nameof(UI_DATA.Network_Disc)) { Source = UI_DATA });

            comboBox_Cluster_Network_Offline.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding(nameof(UI_DATA.Network_Offline), UI_DATA));
            comboBox_Cluster_Network_LanOnly.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding(nameof(UI_DATA.Network_LanOnly), UI_DATA));

            comboBox_Cluster_Misc_Console.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding(nameof(UI_DATA.Misc_Console), UI_DATA));
            comboBox_Cluster_Misc_Mods.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding(nameof(UI_DATA.Misc_Mods), UI_DATA));

            comboBox_Cluster_Shard_Enabled.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding(nameof(UI_DATA.Shard_Enabled), UI_DATA));
            textBox_Cluster_Shard_BindIP.SetBinding(TextBox.TextProperty, new Binding(nameof(UI_DATA.Shard_BindIP)) { Source = UI_DATA });
            textBox_Cluster_Shard_MasterIP.SetBinding(TextBox.TextProperty, new Binding(nameof(UI_DATA.Shard_MasterIP)) { Source = UI_DATA });
            textBox_Cluster_Shard_MasterPort.SetBinding(TextBox.TextProperty, new Binding(nameof(UI_DATA.Shard_MasterPort)) { Source = UI_DATA });
            textBox_Cluster_Shard_MasterKey.SetBinding(TextBox.TextProperty, new Binding(nameof(UI_DATA.Shard_MasterKey)) { Source = UI_DATA });

            #endregion

            //服务器文件路径列表
            dataGrid_LocalServer_ServersPath.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI_DATA.ServerFileListTable_Local)) { Source = UI_DATA });
            dataGrid_CloudServer_ServersPath.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI_DATA.ServerFileListTable_Cloud)) { Source = UI_DATA });

            //远程服务器连接列表
            dataGrid_CloudServer_Connections.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI_DATA.ServerConnectsTable_Cloud)) { Source = UI_DATA });

            //集群服务器信息列表
            dataGrid_ClusterInfo_ServersList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI_DATA.ClusterServersTable)) { Source = UI_DATA });
            dataGrid_ClusterInfo_ServerLevel.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI_DATA.ClusterServersLevel)) { Source = UI_DATA });

            //服务器控制命令列表
            dataGrid_ServersInfo_CommandLine.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI_DATA.ServerConsole)) { Source = UI_DATA });

            //存档文件夹列表
            ComboBox_LocalServer_SavesFolder.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI_DATA.SaveFolders_Local)) { Source = UI_DATA });
            ComboBox_CloudServer_SavesFolder.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI_DATA.SaveFolders_Cloud)) { Source = UI_DATA });
        }

        #region 获取用户配置文件数据

        DataTable ServerFileListTable_CloudOrigin = new DataTable("ServerFileListTable_CloudOrigin");
        /// <summary>
        /// 获取用户数据
        /// </summary>
        private void GetUserData(SQLiteHelper userDataSQLite)
        {

            userDataSQLite.OpenSQLite(appStartupPath + @"\DSTServerManager.db");
            CreateDefaultTable(ref userDataSQLite);

            //读取数据库数据
            UI_DATA.ServerFileListTable_Local = userDataSQLite.ExecuteDataTable("LocalServerList");
            ServerFileListTable_CloudOrigin = userDataSQLite.ExecuteDataTable("CloudServerList");
            UI_DATA.ServerConnectsTable_Cloud = userDataSQLite.ExecuteDataTable("CloudServerConnList");

            UI_DATA.ServerConsole = userDataSQLite.ExecuteDataTable("ServerConsole");
            UI_DATA.ClusterServersLevel = userDataSQLite.ExecuteDataTable("ServerLeveled");

            //读取并合并外部数据
            if (!File.Exists(appStartupPath + @"\DSTServerManager.xlsx")) return;

            ExcelHelper userDataExcel = new ExcelHelper();
            userDataExcel.OpenExcel(appStartupPath + @"\DSTServerManager.xlsx", ExcelEngines.ACE, ExcelVersion.Office2007);

            UI_DATA.ServerFileListTable_Local.MergeExcelData(userDataExcel, "LocalServerList");
            ServerFileListTable_CloudOrigin.MergeExcelData(userDataExcel, "CloudServerList");
            UI_DATA.ServerConnectsTable_Cloud.MergeExcelData(userDataExcel, "CloudServerConnList");

            UI_DATA.ServerConsole.MergeExcelData(userDataExcel, "ServerConsole");
            UI_DATA.ClusterServersLevel.MergeExcelData(userDataExcel, "ServerLeveled");

            //更新本地数据库数据
            userDataSQLite.SaveDataTable(UI_DATA.ServerFileListTable_Local, "LocalServerList");
            userDataSQLite.SaveDataTable(ServerFileListTable_CloudOrigin, "CloudServerList");
            userDataSQLite.SaveDataTable(UI_DATA.ServerConnectsTable_Cloud, "CloudServerConnList");

            userDataSQLite.SaveDataTable(UI_DATA.ServerConsole, "ServerConsole");
            userDataSQLite.SaveDataTable(UI_DATA.ClusterServersLevel, "ServerLeveled");
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
            userDataSQLite.CreatDataTable("LocalServerList", parameter);
            userDataSQLite.CreatDataTable("CloudServerList", parameter);

            parameter = new string[5] { "ID integer primary key", "IP text", "UserName text", "Password text", "ServerID text" };
            userDataSQLite.CreatDataTable("CloudServerConnList", parameter);

            parameter = new string[5] { "ID integer primary key", "Type text", "Explain text", "Command text", "Parameter text" };
            userDataSQLite.CreatDataTable("ServerConsole", parameter);

            parameter = new string[8] { "ID integer primary key", "Name text", "English text", "Chinese text", "WorldType text", "Type text", "Enum text", "Current text" };
            userDataSQLite.CreatDataTable("ServerLeveled", parameter);
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
