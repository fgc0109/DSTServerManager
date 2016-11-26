using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DSTServerManager.Servers
{
    class ServerConsole
    {
        static public DataTable GetServerConsoleCommandData()
        {
            DataTable serverData = new DataTable("ServerCommandData");

            //尝试读取配置数据表格
            //
            //return serverData;

            //读取程序内置配置
            serverData.Columns.Add(new DataColumn("ID", typeof(int)));
            serverData.Columns.Add(new DataColumn("Name", typeof(string)));
            serverData.Columns.Add(new DataColumn("Command", typeof(string)));
            serverData.Columns.Add(new DataColumn("Parameter", typeof(string)));

            for (int i = 0; i < 4; i++)
                serverData.Columns[i].ReadOnly = true;

            serverData.Rows.Add(new object[4] { 1, "立即保存", "c_save()", "None" });
            serverData.Rows.Add(new object[4] { 2, "关闭服务器", "c_shutdown()", "None" });

            return serverData;
        }
    }
}
