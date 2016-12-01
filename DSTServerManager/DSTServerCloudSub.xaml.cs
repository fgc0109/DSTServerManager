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
using Renci.SshNet;
using System.Windows.Shapes;

namespace DSTServerManager
{
    /// <summary>
    /// DSTServerCloudSub.xaml 的交互逻辑
    /// </summary>
    public partial class DSTServerCloudSub : Window
    {
        public delegate void PassValuesHandler(object sender, PassValuesEventArgs e);
        public event PassValuesHandler PassValuesEvent;

        public DSTServerCloudSub(string ip, string user, string pass)
        {
            InitializeComponent();

            if (ip != null) textBox_Addr.Text = ip;
            if (user != null) textBox_User.Text = user;
            if (pass != null) textBox_Pass.Text = pass;
        }

        private void button_SubWin_ConnectionCheck_Click(object sender, RoutedEventArgs e)
        {
            SftpClient sftpclient = new SftpClient(textBox_Addr.Text, textBox_User.Text, textBox_Pass.Text);

            try
            {
                sftpclient.Connect();
                button_SubWin_ConnectionSaved.IsEnabled = true;
                label_Logs.Content += "连接测试成功!";
            }
            catch (Exception ex)
            {
                button_SubWin_ConnectionSaved.IsEnabled = false;
                label_Logs.Content += ex.ToString();
            }
            finally
            {
                label_Logs.Content += "\r\n";
                sftpclient = null;
            }

            //List<string> saveFolders_Cloud = SavesManager.GetSavesFolder(sftpclient);
            //foreach (var item in saveFolders_Cloud)
            //    comboBox_SavesFolder_Cloud.Items.Add(item);
            //comboBox_SavesFolder_Cloud.SelectedIndex = 0;
        }

        private void button_SubWin_ConnectionSaved_Click(object sender, RoutedEventArgs e)
        {
            PassValuesEventArgs args = new PassValuesEventArgs(textBox_Addr.Text, textBox_User.Text, textBox_Pass.Text);
            PassValuesEvent(this, args);

            Close();
        }
    }

    /// <summary>
    /// 容纳参数传递事件的附加信息
    /// </summary>
    public class PassValuesEventArgs : EventArgs
    {
        private readonly String m_Addr;
        private readonly String m_user;
        private readonly String m_pass;

        public PassValuesEventArgs(String addr, String user, String pass)
        {
            m_Addr = addr;
            m_user = user;
            m_pass = pass;
        }

        public String Addr
        {
            get { return m_Addr; }
        }
        public String User
        {
            get { return m_user; }
        }
        public String Pass
        {
            get { return m_pass; }
        }
    }

}
