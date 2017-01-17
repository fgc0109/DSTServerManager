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
            dataGrid_ClusterInfo_ServerLevel.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.ServerLevel)) { Source = UI });

            //服务器控制命令列表
            dataGrid_ServersInfo_CommandLine.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.CommandLine)) { Source = UI });

            //存档文件夹列表
            ComboBox_LocalServer_SavesFolder.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.SaveFolders_Local)) { Source = UI });
            ComboBox_CloudServer_SavesFolder.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.SaveFolders_Cloud)) { Source = UI });

            //当前选定的远程连接信息
            dataGrid_CloudServer_Connections.SetBinding(DataGrid.SelectedItemProperty, new Binding(nameof(UI.CurrentConn)) { Source = UI });

            //当前服务器模组文件信息
            dataGrid_Modifications_MainInfos.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.Modifications)) { Source = UI });
            dataGrid_Modifications_Configura.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(UI.Configuration)) { Source = UI });
        }

        #region 获取用户配置文件数据

        private static DataTable ConnectionsOrigin = null;
        private static DataTable ServerCloudOrigin = null;
        private static DataTable ServerLocalOrigin = null;
        private static DataTable CommandLineOrigin = null;
        private static DataTable ServerLevelOrigin = null;

        /// <summary>
        /// 获取用户数据
        /// </summary>
        private void GetDatabaseData()
        {
            ServerCloudOrigin = SQLiteHelper.ExecuteDataTable(nameof(UI.ServerCloud));
            ServerLocalOrigin = SQLiteHelper.ExecuteDataTable(nameof(UI.ServerLocal));
            ConnectionsOrigin = SQLiteHelper.ExecuteDataTable(nameof(UI.Connections));
            CommandLineOrigin = SQLiteHelper.ExecuteDataTable(nameof(UI.CommandLine));
            ServerLevelOrigin = SQLiteHelper.ExecuteDataTable(nameof(UI.ServerLevel));



            //更新本地数据库数据
            SQLiteHelper.SaveDataTable(ConnectionsOrigin, nameof(UI.Connections));
            SQLiteHelper.SaveDataTable(ServerCloudOrigin, nameof(UI.ServerCloud));
            SQLiteHelper.SaveDataTable(ServerLocalOrigin, nameof(UI.ServerLocal));

            SQLiteHelper.SaveDataTable(CommandLineOrigin, nameof(UI.CommandLine));
            SQLiteHelper.SaveDataTable(ServerLevelOrigin, nameof(UI.ServerLevel));
        }

        /// <summary>
        /// 创建默认的数据表
        /// </summary>
        static private void CreateDefaultTable()
        {
            //创建默认的数据表结构
            string[] parameter = null;
            parameter = new string[3] { "ID integer primary key", "Type text", "Path text" };
            SQLiteHelper.CreatDataTable(nameof(UI.ServerCloud), parameter);
            SQLiteHelper.CreatDataTable(nameof(UI.ServerLocal), parameter);

            parameter = new string[5] { "ID integer primary key", "IP text", "UserName text", "Password text", "ServerID text" };
            SQLiteHelper.CreatDataTable(nameof(UI.Connections), parameter);

            parameter = new string[5] { "ID integer primary key", "Type text", "Explain text", "Command text", "Parameter text" };
            SQLiteHelper.CreatDataTable(nameof(UI.CommandLine), parameter);

            parameter = new string[8] { "ID integer primary key", "Name text", "English text", "Chinese text", "WorldType text", "Type text", "Enum text", "Current text" };
            SQLiteHelper.CreatDataTable(nameof(UI.ServerLevel), parameter);
        }

        /// <summary>
        /// 读取外部数据表
        /// </summary>
        static private void GetExcelData()
        {
            //读取并合并外部数据
            if (!File.Exists(appStartupPath + @"\DSTServerManager.xlsx")) return;

            ExcelHelper userDataExcel = new ExcelHelper();
            userDataExcel.OpenExcel(appStartupPath + @"\DSTServerManager.xlsx", ExcelEngines.ACE, ExcelVersion.Office2007);

            ConnectionsOrigin.MergeExcelData(userDataExcel.ExecuteDataTable(nameof(UI.Connections)));
            ServerCloudOrigin.MergeExcelData(userDataExcel.ExecuteDataTable(nameof(UI.ServerCloud)));
            ServerLocalOrigin.MergeExcelData(userDataExcel.ExecuteDataTable(nameof(UI.ServerLocal)));

            CommandLineOrigin.MergeExcelData(userDataExcel.ExecuteDataTable(nameof(UI.CommandLine)));
            ServerLevelOrigin.MergeExcelData(userDataExcel.ExecuteDataTable(nameof(UI.ServerLevel)));
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
