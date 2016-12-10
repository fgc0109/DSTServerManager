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

namespace DSTServerManager
{
    /// <summary>
    /// SubWindow_SteamCommandLine.xaml 的交互逻辑
    /// </summary>
    public partial class SubWindow_SteamCommand : Window
    {
        string appStartupPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public delegate void SteamCommandHandler(object sender, SteamCommandEventArgs e);
        public event SteamCommandHandler SteamCommandEvent;

        private StreamWriter m_StreamWriter = null;
        private Process m_ServerProcess = null;

        public SubWindow_SteamCommand()
        {
            InitializeComponent();
            textBox_Path.Text = appStartupPath;
            //textBox_Path.Text = @"D:\Data\dst";
            m_ServerProcess = new Process();

            textBox_CMDLog.Text += "SteamCMD没有立刻将输出缓存写入数据流\r\n";
            textBox_CMDLog.Text += "暂时不使用内置窗口\r\n";
        }

        private void button_Download_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder command = new StringBuilder(512);
            command.Append($" +force_install_dir {textBox_Path.Text.Replace(@"\", @"\\")}");
            command.Append($" +login anonymous");
            command.Append($" +app_update 343050");
            if ((bool)radioButton_Alpha.IsChecked) command.Append($" -beta {textBox_Name.Text}");
            command.Append($" +validate");
            command.Append($" +quit");

            StartProcess(command.ToString());
        }

        public void StartProcess(params string[] argument)
        {
            //获取工作目录
            string directory = string.Empty;
            string[] folder = appStartupPath.Split('\\');
            for (int i = 0; i < folder.Length - 1; i++) directory += folder[i] + "\\";

            //获取完整命令
            string command = string.Empty;
            foreach (var argue in argument) command += argue;

            //设置启动参数
            m_ServerProcess.StartInfo.WorkingDirectory = directory;
            m_ServerProcess.StartInfo.FileName = appStartupPath + "\\steamcmd.exe";
            m_ServerProcess.StartInfo.Arguments = command;
            m_ServerProcess.EnableRaisingEvents = true;
            m_ServerProcess.Exited += new EventHandler(process_Exited);

            m_ServerProcess.Start();
        }

        private void process_Exited(object sender, EventArgs e)
        {
            SteamCommandEventArgs args = new SteamCommandEventArgs(textBox_Path.Text.Replace(@"\", @"\\"));

            SteamCommandEvent(this, args);
        }

        public void SendCommand(string command)
        {
            m_StreamWriter = m_ServerProcess.StandardInput;

            m_StreamWriter.WriteLine(command);
            m_StreamWriter.Flush();
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

        //https://developer.valvesoftware.com/wiki/Dedicated_Servers_List
    }

    /// <summary>
    /// 容纳参数传递事件的附加信息
    /// </summary>
    public class SteamCommandEventArgs : EventArgs
    {
        private readonly string m_NewServerPath;

        public SteamCommandEventArgs(string NewServerPath)
        {
            m_NewServerPath = NewServerPath;
        }
        public bool IsNewRow { get { return m_NewServerPath; } }
    }
}
