using DSTServerManager.DataHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTServerManager
{
    static class DatabaseManager
    {
        /// <summary>
        /// 创建默认的数据表
        /// </summary>
        /// <param name="userDataSQLite"></param>
        /// <param name="exception"></param>
        static public void CreateDefaultTable(ref SQLiteHelper userDataSQLite, out string exception)
        {
            exception = string.Empty;
            //创建默认的数据表结构
            string[] parameter = null;
            parameter = new string[3] { "ID integer primary key", "Type text", "Path text" };
            userDataSQLite.CreatDataTable("LocalServerList", parameter, out exception);
            userDataSQLite.CreatDataTable("CloudServerList", parameter, out exception);

            parameter = new string[4] { "ID integer primary key", "IP text", "User text", "Password text" };
            userDataSQLite.CreatDataTable("CloudServerConnList", parameter, out exception);
        }
    }
}
