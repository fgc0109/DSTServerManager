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
using DSTServerManager.DataHelper;

namespace DSTServerManager
{
    public partial class DSTServerLauncher : Window
    {
        /// <summary>
        /// 更新集群服务器信息
        /// </summary>
        /// <param name="keyword">筛选关键词</param>
        private void RefreshClusterData(string keyword)
        {
            //获取集群文件夹名称并更新显示
            m_ClusterFolder = SavesManager.GetClusterFolder(comboBox_SavesFolder.SelectedItem?.ToString(), keyword);
            listBox_Cluster.Items.Clear();
            foreach (var item in m_ClusterFolder)
            {
                if (!listBox_Cluster.Items.Contains(item))
                    listBox_Cluster.Items.Add(item);
            }

            //获取集群信息
            m_ClusterInfo = SavesManager.GetClusterInfo(comboBox_SavesFolder.SelectedItem?.ToString(), keyword, dataGrid_Cluster_Servers.Columns.Count);
            if (listBox_Cluster.Items.Count != 0)
                listBox_Cluster.SelectedIndex = 0;
        }

        /// <summary>
        /// 更新集群下所有服务器信息
        /// </summary>
        private void RefreshServersData()
        {
            int index = listBox_Cluster.SelectedIndex;
            if (index == -1)
                return;

            //将当前选定的集群信息赋值给界面绑定的类实例
            CopyHelper.CopyAllProperties(m_ClusterInfo[index].Setting, m_UserInterfaceIniData);
            if (m_ClusterInfo[index].Servers.Count != 0)
                dataGrid_Cluster_Servers.SelectedIndex = 0;

            //更新当前选定的集群服务器DataGrid信息
            m_UserInterfaceServerData.ClusterServerTable.Clear();
            for (int i = 0; i < m_ClusterInfo[index].Servers.Count; i++)
            {
                m_UserInterfaceServerData.ClusterServerTable.Rows.Add(m_UserInterfaceServerData.ClusterServerTable.NewRow());
                for (int j = 0; j < m_UserInterfaceServerData.ClusterServerTable.Columns.Count; j++)
                    m_UserInterfaceServerData.ClusterServerTable.Rows[i][j] = m_ClusterInfo[index].ClusterServerTable.Rows[i][j];
            }
        }

        /// <summary>
        /// 创建子文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        private void CreatChildFolder(string path,string name)
        {

        }
    }
}
