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
using System.Diagnostics;
using System.Data.OleDb;
using System.Globalization;
using System.Data;
using Renci.SshNet;
using DSTServerManager.DataHelper;

namespace DSTServerManager
{
    public partial class DSTServerLauncher : Window
    {
        /// <summary>
        /// 更新指定存档路径下的集群服务器列表
        /// </summary>
        /// <param name="keyword">筛选关键词</param>
        /// <param name="saveFolder">存档文件夹名</param>
        /// <param name="clusterList">Listbox控件</param>
        /// <param name="client">远程服务器连接</param>
        private void RefreshClusterData(string saveFolder, string keyword, ref ListBox clusterList, SftpClient client)
        {
            //获取集群文件夹名称并更新显示
            List<string> clusterFolder = null;
            if (client == null) clusterFolder = SavesManager.GetClusterFolder(saveFolder, keyword);
            else clusterFolder = SavesManager.GetClusterFolder(saveFolder, keyword, client);

            clusterList.Items.Clear();
            foreach (var item in clusterFolder)
                if (!clusterList.Items.Contains(item)) clusterList.Items.Add(item);
        }

        /// <summary>
        /// 更新集群下所有服务器信息
        /// </summary>
        /// <param name="clusterInfo">当前选定的集群信息</param>
        /// <param name="bindData">界面绑定的集群</param>
        private void RefreshServersData(ClusterInfo clusterInfo, ref UserInterfaceData bindData)
        {
            //将当前选定的集群信息赋值给界面绑定的类实例
            ExtendHelper.CopyAllProperties(clusterInfo.ClusterSetting, bindData);

            //更新当前选定的集群服务器DataGrid信息
            bindData.ClusterServersTable.Clear();
            ExtendHelper.CopyAllProperties(clusterInfo, bindData);
        }


        /// <summary>
        /// 创建子文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        private void CreatChildFolder(string path, string name)
        {

        }
    }
}
