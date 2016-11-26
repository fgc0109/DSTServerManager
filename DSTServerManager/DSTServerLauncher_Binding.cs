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

namespace DSTServerManager
{
    public partial class DSTServerLauncher : Window
    {
        private void BindingState()
        {
            textBox_Cluster_Gameplay_Mode.SetBinding(TextBox.TextProperty, new Binding("Gameplay_Mode") { Source = m_UserInterfaceIniData });
            textBox_Cluster_Gameplay_Player.SetBinding(TextBox.TextProperty, new Binding("Gameplay_Player") { Source = m_UserInterfaceIniData });
            comboBox_Cluster_Gameplay_PVP.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding("Gameplay_PVP", m_UserInterfaceIniData));
            comboBox_Cluster_Gameplay_Pause.SetBinding(ComboBox.SelectedIndexProperty, new Binding("Gameplay_Pause") { Source = m_UserInterfaceIniData });

            textBox_Cluster_Network_Name.SetBinding(TextBox.TextProperty, new Binding("Network_Name") { Source = m_UserInterfaceIniData });
            textBox_Cluster_Network_Pass.SetBinding(TextBox.TextProperty, new Binding("Network_Pass") { Source = m_UserInterfaceIniData });
            textBox_Cluster_Network_Intention.SetBinding(TextBox.TextProperty, new Binding("Network_Intention") { Source = m_UserInterfaceIniData });
            textBox_Cluster_Network_TickRate.SetBinding(TextBox.TextProperty, new Binding("Network_TickRate") { Source = m_UserInterfaceIniData });
            textBox_Cluster_Network_Timeout.SetBinding(TextBox.TextProperty, new Binding("Network_Timeout") { Source = m_UserInterfaceIniData });
            textBox_Cluster_Network_Disc.SetBinding(TextBox.TextProperty, new Binding("Network_Disc") { Source = m_UserInterfaceIniData });

            comboBox_Cluster_Network_Offline.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding("Network_Offline", m_UserInterfaceIniData));
            comboBox_Cluster_Network_LanOnly.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding("Network_LanOnly", m_UserInterfaceIniData));

            comboBox_Cluster_Misc_Console.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding("Misc_Console", m_UserInterfaceIniData));
            comboBox_Cluster_Misc_Mods.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding("Misc_Mods", m_UserInterfaceIniData));

            comboBox_Cluster_Shard_Enabled.SetBinding(ComboBox.SelectedIndexProperty, SetBoolBinding("Shard_Enabled", m_UserInterfaceIniData));
            textBox_Cluster_Shard_BindIP.SetBinding(TextBox.TextProperty, new Binding("Shard_BindIP") { Source = m_UserInterfaceIniData });
            textBox_Cluster_Shard_MasterIP.SetBinding(TextBox.TextProperty, new Binding("Shard_MasterIP") { Source = m_UserInterfaceIniData });
            textBox_Cluster_Shard_MasterPort.SetBinding(TextBox.TextProperty, new Binding("Shard_MasterPort") { Source = m_UserInterfaceIniData });
            textBox_Cluster_Shard_MasterKey.SetBinding(TextBox.TextProperty, new Binding("Shard_MasterKey") { Source = m_UserInterfaceIniData });

            dataGrid_Cluster_Servers.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("ClusterServerTable") { Source = m_UserInterfaceServerData });
        }

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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value) return 1;
            else return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value == 1) return true;
            else return false;
        }
    }
}
