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
using System.Windows.Shapes;

namespace DSTServerManager
{
    /// <summary>
    /// SubWindow_SteamCommandLine.xaml 的交互逻辑
    /// </summary>
    public partial class SubWindow_SteamCommand : Window
    {
        public delegate void SteamCommandHandler(object sender, SteamCommandEventArgs e);
        public event SteamCommandHandler SteamCommandEvent;

        public SubWindow_SteamCommand()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    /// 容纳参数传递事件的附加信息
    /// </summary>
    public class SteamCommandEventArgs : EventArgs
    {
        private readonly bool m_NewRow;

        public SteamCommandEventArgs(bool newRow)
        {
            m_NewRow = newRow;
        }
        public bool IsNewRow { get { return m_NewRow; } }
    }
}
