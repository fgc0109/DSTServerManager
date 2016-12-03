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
using System.Globalization;
using System.IO;
using DSTServerManager.DataHelper;
using System.Data.SQLite;
using System.Data;

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

            textBox_Cluster_Gameplay_Mode.SetBinding(TextBox.TextProperty, new Binding("Gameplay_Mode") { Source = UI_DATA });
            textBox_Cluster_Gameplay_Player.SetBinding(TextBox.TextProperty, new Binding("Gameplay_Player") { Source = UI_DATA });
            comboBox_Cluster_Gameplay_PVP.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding("Gameplay_PVP", UI_DATA));
            comboBox_Cluster_Gameplay_Pause.SetBinding(ComboBox.SelectedIndexProperty, new Binding("Gameplay_Pause") { Source = UI_DATA });
            comboBox_Cluster_Gameplay_Vote.SetBinding(ComboBox.SelectedIndexProperty, new Binding("Gameplay_Vote") { Source = UI_DATA });

            textBox_Cluster_Network_Name.SetBinding(TextBox.TextProperty, new Binding("Network_Name") { Source = UI_DATA });
            textBox_Cluster_Network_Pass.SetBinding(TextBox.TextProperty, new Binding("Network_Pass") { Source = UI_DATA });
            textBox_Cluster_Network_Intention.SetBinding(TextBox.TextProperty, new Binding("Network_Intention") { Source = UI_DATA });
            textBox_Cluster_Network_TickRate.SetBinding(TextBox.TextProperty, new Binding("Network_TickRate") { Source = UI_DATA });
            textBox_Cluster_Network_Timeout.SetBinding(TextBox.TextProperty, new Binding("Network_Timeout") { Source = UI_DATA });
            textBox_Cluster_Network_Disc.SetBinding(TextBox.TextProperty, new Binding("Network_Disc") { Source = UI_DATA });

            comboBox_Cluster_Network_Offline.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding("Network_Offline", UI_DATA));
            comboBox_Cluster_Network_LanOnly.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding("Network_LanOnly", UI_DATA));

            comboBox_Cluster_Misc_Console.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding("Misc_Console", UI_DATA));
            comboBox_Cluster_Misc_Mods.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding("Misc_Mods", UI_DATA));

            comboBox_Cluster_Shard_Enabled.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding("Shard_Enabled", UI_DATA));
            textBox_Cluster_Shard_BindIP.SetBinding(TextBox.TextProperty, new Binding("Shard_BindIP") { Source = UI_DATA });
            textBox_Cluster_Shard_MasterIP.SetBinding(TextBox.TextProperty, new Binding("Shard_MasterIP") { Source = UI_DATA });
            textBox_Cluster_Shard_MasterPort.SetBinding(TextBox.TextProperty, new Binding("Shard_MasterPort") { Source = UI_DATA });
            textBox_Cluster_Shard_MasterKey.SetBinding(TextBox.TextProperty, new Binding("Shard_MasterKey") { Source = UI_DATA });

            #endregion

            //服务器列表
            dataGrid_ClusterInfo_ServersList.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("ClusterServersTable") { Source = UI_DATA });

            //服务器文件路径列表
            dataGrid_LocalServer_ServersPath.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("ServerFileListTable_Local") { Source = UI_DATA });
            dataGrid_CloudServer_ServersPath.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("ServerFileListTable_Cloud") { Source = UI_DATA });

            //远程服务器连接列表
            dataGrid_CloudServer_Connection.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("ServerConnectsTable_Cloud") { Source = UI_DATA });

            //服务器控制台命令列表
            dataGrid_Server_Command.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("ServerConsole") { Source = UI_DATA });
            dataGrid_ClusterInfo_ServerLevel.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("ServerLeveled") { Source = UI_DATA });
        }

        #region 获取用户配置文件数据

        /// <summary>
        /// 获取用户数据
        /// </summary>
        private void GetUserData(SQLiteHelper userDataSQLite)
        {
            textBox_BasicInfo_Key.Text = ConfigHelper.GetValue("textBox_BasicInfo_Key");

            string exception = string.Empty;
            dataGrid_LocalServer_ServersPath.FrozenColumnCount = 2;
            dataGrid_CloudServer_ServersPath.FrozenColumnCount = 2;

            userDataSQLite.OpenSQLite(appStartupPath + @"\DSTServerManager.db", out exception);
            CreateDefaultTable(ref userDataSQLite, out exception);

            //读取数据库数据
            UI_DATA.ServerFileListTable_Local = userDataSQLite.ExecuteDataTable("LocalServerList", out exception);
            UI_DATA.ServerFileListTable_Cloud = userDataSQLite.ExecuteDataTable("CloudServerList", out exception);
            UI_DATA.ServerConnectsTable_Cloud = userDataSQLite.ExecuteDataTable("CloudServerConnList", out exception);

            UI_DATA.ServerConsole = userDataSQLite.ExecuteDataTable("ServerConsole", out exception);
            UI_DATA.ServerLeveled = userDataSQLite.ExecuteDataTable("ServerLeveled", out exception);           

            //读取并合并外部数据
            if (!File.Exists(appStartupPath + @"\DSTServerManager.xlsx")) return;

            ExcelHelper userDataExcel = new ExcelHelper();
            userDataExcel.OpenExcel(appStartupPath + @"\DSTServerManager.xlsx", ExcelEngines.ACE, ExcelVersion.Office2007, out exception);

            UI_DATA.ServerFileListTable_Local.MergeExcelData(userDataExcel, "LocalServerList", out exception);
            UI_DATA.ServerFileListTable_Cloud.MergeExcelData(userDataExcel, "CloudServerList", out exception);
            UI_DATA.ServerConnectsTable_Cloud.MergeExcelData(userDataExcel, "CloudServerConnList", out exception);

            UI_DATA.ServerConsole.MergeExcelData(userDataExcel, "ServerConsole", out exception);
            UI_DATA.ServerLeveled.MergeExcelData(userDataExcel, "ServerLeveled", out exception);

            //更新本地数据库数据
            userDataSQLite.SaveDataTable(UI_DATA.ServerFileListTable_Local, "LocalServerList", out exception);
            userDataSQLite.SaveDataTable(UI_DATA.ServerFileListTable_Cloud, "CloudServerList", out exception);
            userDataSQLite.SaveDataTable(UI_DATA.ServerConnectsTable_Cloud, "CloudServerConnList", out exception);

            userDataSQLite.SaveDataTable(UI_DATA.ServerConsole, "ServerConsole", out exception);
            userDataSQLite.SaveDataTable(UI_DATA.ServerLeveled, "ServerLeveled", out exception);
        }

        #endregion

        #region 默认的数据库字段

        /// <summary>
        /// 创建默认的数据表
        /// </summary>
        /// <param name="userDataSQLite"></param>
        /// <param name="exception"></param>
        static private void CreateDefaultTable(ref SQLiteHelper userDataSQLite, out string exception)
        {
            exception = string.Empty;
            //创建默认的数据表结构
            string[] parameter = null;
            parameter = new string[3] { "ID integer primary key", "Type text", "Path text" };
            userDataSQLite.CreatDataTable("LocalServerList", parameter, out exception);
            userDataSQLite.CreatDataTable("CloudServerList", parameter, out exception);

            parameter = new string[5] { "ID integer primary key", "IP text", "UserName text", "Password text", "ServerID text" };
            userDataSQLite.CreatDataTable("CloudServerConnList", parameter, out exception);

            parameter = new string[4] { "ID integer primary key", "Explain text", "Command text", "Parameter text" };
            userDataSQLite.CreatDataTable("ServerConsole", parameter, out exception);

            parameter = new string[8] { "ID integer primary key", "Name text", "English text", "Chinese text", "WorldType text", "Type text", "Enum text", "Current text" };
            userDataSQLite.CreatDataTable("ServerLeveled", parameter, out exception);
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
