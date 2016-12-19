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

            string userPath = (m_UserName == "root") ? "/root" : $"/home/{m_UserName}";
            textBox_Path.Text = userPath + "/DontStarveDedicatedServer";

            StartScreens();
            ShellBuilder();
        }

        private void Button_Download_Click(object sender, RoutedEventArgs e)
        {
            string userPath = (m_UserName == "root") ? "/root" : $"/home/{m_UserName}";
            List<string> path = new List<string>();

            try { m_SftpClient.CreateDirectory(textBox_Path.Text); }
            catch { }

            SendCommand($"{userPath}/steamcmd/getsteamcmd.sh");
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

        public void SendCommand(string command)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(command + "\r");
            m_ShellStream.WriteAsync(buffer, 0, buffer.Length);

            m_ShellStream.FlushAsync();
        }

        private void Screens_OutputDataReceived(object sender, ShellDataEventArgs received)
        {
            string data = Encoding.UTF8.GetString(received.Data);
            data = color.Replace(data, "");
            data = lines.Replace(data, "");
            data = enter.Replace(data, "");
            data = othr1.Replace(data, "");
            data = othr2.Replace(data, "");
            screensLogBuffer.Append(data);

            if (isLogOutput == false) return;
            DisplayData();
        }

        private void textBox_Path_TextChanged(object sender, TextChangedEventArgs e)
        {
            string userPath = (m_UserName == "root") ? "/root" : $"/home/{m_UserName}";

            if (!textBox_Path.Text.Contains(userPath + "/")) button_Download.IsEnabled = false;
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

        public void DisplayData()
        {
            string connectLog = screensLogBuffer.ToString();
            screensLogBuffer.Clear();

            Dispatcher.Invoke(new Action(() =>
              {
                  textBox_CMDLog.Text += connectLog;
                  connectLog = string.Empty;
                  textBox_CMDLog.CaretIndex = textBox_CMDLog.Text.Length;
                  textBox_CMDLog.ScrollToEnd();
              }));
        }

        private void ShellBuilder()
        {
            bool isExist = false;
            string userPath = (m_UserName == "root") ? "/root" : $"/home/{m_UserName}";
            List<string> path = new List<string>();
            List<string> file = new List<string>();

            foreach (var item in m_SftpClient.ListDirectory(userPath))
                if (item.IsDirectory) path.Add(item.Name);
            if (!path.Contains("steamcmd")) m_SftpClient.CreateDirectory(userPath + "/steamcmd");

            foreach (var item in m_SftpClient.ListDirectory(userPath + "/steamcmd"))
                if (!item.IsDirectory) file.Add(item.Name);
            if (file.Contains("steamcmd_linux.tar.gz")) isExist = true;

            m_SftpClient.Create(userPath + "/steamcmd" + "/getsteamcmd.sh");

            StringBuilder command = new StringBuilder(512);
            command.Append($" +force_install_dir {textBox_Path.Text.Replace(@"\", @"\\")}");
            command.Append($" +login anonymous");
            command.Append($" +app_update 343050");
            if ((bool)radioButton_Alpha.IsChecked) command.Append($" -beta {textBox_Name.Text}");
            command.Append($" validate");
            command.Append($" +quit");

            MemoryStream commandStream = new MemoryStream();
            StreamWriter commandWriter = new StreamWriter(commandStream, Encoding.UTF8);

            commandWriter.Write($"#!/bin/bash\n\n");
            commandWriter.Write($"cd steamcmd\n");
            if (!isExist) commandWriter.Write($"wget https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz\n");
            if (!isExist) commandWriter.Write($"tar -xvzf steamcmd_linux.tar.gz\n");
            //commandWriter.Write($"rm -f steamcmd_linux.tar.gz\n");
            commandWriter.Write($"chmod 777 ./steamcmd.sh\n");
            commandWriter.Write($"./steamcmd.sh {command.ToString()}\n");

            commandWriter.Flush();

            m_SftpClient.WriteAllBytes(userPath + "/steamcmd" + "/getsteamcmd.sh", commandStream.ToArray());
            commandStream.Close();

            SendCommand($"chmod 777 {userPath}/steamcmd/getsteamcmd.sh");
            SendCommand($"dos2unix {userPath}/steamcmd/getsteamcmd.sh");

            m_SftpClient.Dispose();
        }
    }
}
