using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Text.RegularExpressions;

namespace DSTServerManager
{
    /// <summary>
    /// SubWindow_SteamCommandLine.xaml 的交互逻辑
    /// </summary>
    public partial class SteamCommand_2 : Window
    {
        string appStartupPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public delegate void SteamCommandHandler(object sender, SteamCommandEventArgs e);
        public event SteamCommandHandler SteamCommandEvent;

        private static Regex color = new Regex("\\[[^ ]*?m", RegexOptions.Compiled);
        private static Regex lines = new Regex("%[ ]*?", RegexOptions.Compiled);
        private static Regex enter = new Regex("[ ]*\\r", RegexOptions.Compiled);
        private static Regex othr1 = new Regex("\\]\\d;[\\s\\S]+", RegexOptions.Compiled);
        private static Regex othr2 = new Regex("[ ]*\\[K[\\s\\S]?", RegexOptions.Compiled);

        private bool isLogOutput = true;
        private ShellStream m_ShellStream = null;
        private StringBuilder screensLogBuffer = new StringBuilder(4096);

        private SftpClient m_SftpClient;
        private SshClient m_SshClient;

        private string m_ScreenName = string.Empty;
        private string m_UserName = string.Empty;
        private string m_Password = string.Empty;
        private string m_Location = string.Empty;

        public SteamCommand_2(string location, string userName, string password)
        {
            InitializeComponent();

            m_SftpClient = new SftpClient(location, 22, userName, password);
            m_SshClient = new SshClient(location, userName, password);

            m_UserName = userName;
            m_Password = password;
            m_Location = location;
        }

        private void Button_Download_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder command = new StringBuilder(512);
            command.Append($" +force_install_dir {textBox_Path.Text.Replace(@"\", @"\\")}");
            command.Append($" +login anonymous");
            command.Append($" +app_update 343050");
            if ((bool)radioButton_Alpha.IsChecked) command.Append($" -beta {textBox_Name.Text}");
            command.Append($" +validate");
            command.Append($" +quit");

            StartScreens();
        }

        /// <summary>
        /// 开启Screens
        /// </summary>
        public void StartScreens()
        {
            try
            {
                if (m_SftpClient.IsConnected == false) m_SftpClient.Connect();
                if (m_SshClient.IsConnected == false) m_SshClient.Connect();
            }
            catch { throw; }

            m_ShellStream = m_SshClient.CreateShellStream("putty-vt100", 80, 24, 800, 600, 4096);
            byte[] buffer = new byte[4096];

            m_ShellStream.DataReceived += new EventHandler<ShellDataEventArgs>(Screens_OutputDataReceived);
            m_ShellStream.ReadAsync(buffer, 0, buffer.Length);
        }

        private void Screens_OutputDataReceived(object sender, ShellDataEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void textBox_Path_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Directory.Exists(textBox_Path.Text)) button_Download.IsEnabled = false;
            else button_Download.IsEnabled = true;
        }

        private void button_SelectPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFolder = new System.Windows.Forms.FolderBrowserDialog();
            openFolder.ShowDialog();
            if (openFolder.SelectedPath != string.Empty) textBox_Path.Text = openFolder.SelectedPath;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //if (m_SteamProcess == null) return;
            //m_SteamProcess.CloseMainWindow();
            //m_SteamProcess.Close();
        }

        //https://developer.valvesoftware.com/wiki/Dedicated_Servers_List
    }
}
