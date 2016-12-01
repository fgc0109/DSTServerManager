using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTServerManager.DataHelper
{
    /// <summary> 
    /// 说明：这是一个针对System.Data.SQLite的数据库常规操作封装的通用类。 
    /// </summary> 
    class SQLiteHelper
    {
        private SQLiteConnection m_dbConnection = null;

        public SQLiteConnection DBConnection
        { get { return m_dbConnection; } }

        private string connectionString = string.Empty;

        /// <summary> 
        /// 尝试打开SQLite连接
        /// </summary> 
        /// <param name="filePath">SQLite数据库文件路径</param>
        /// <param name="exception">异常信息</param>
        public bool OpenSQLite(string filePath, out string exception)
        {
            m_dbConnection = new SQLiteConnection("Data Source=" + filePath);

            try
            {
                m_dbConnection.Open();
                exception = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }

        /// <summary> 
        /// 执行一个查询语句，返回一个包含查询结果的DataTable 
        /// </summary> 
        /// <param name="tableName"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string tableName, out string exception)
        {
            DataTable dataTable = new DataTable(tableName);
            exception = string.Empty;

            try
            {
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter($"select * from {tableName};", m_dbConnection);
                dataAdapter.Fill(dataTable);
            }
            catch (Exception ex) { exception = ex.ToString(); }
            return dataTable;
        }



        public bool CreatDataTable(string tableName, object[] parameters, out string exception)
        {
            StringBuilder commandBuilder = new StringBuilder();
            if (parameters != null)
            {
                foreach (var item in parameters) commandBuilder.Append(item).Append(", ");
                commandBuilder.Remove(commandBuilder.Length - 2, 2);
            }
            string command = $"create table if not exists '{tableName}'({commandBuilder.ToString()});";
            exception = string.Empty;

            try
            {
                SQLiteCommand cmdCreateTable = new SQLiteCommand(command, m_dbConnection);
                cmdCreateTable.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return false;
            }
        }
    }
}
