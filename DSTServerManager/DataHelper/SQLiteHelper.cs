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
    /// 操作SQLite数据库
    /// </summary> 
    public static class SQLiteHelper
    {
        private static SQLiteConnection connection = null;

        public static SQLiteConnection DBConnection
        { get { return connection; } }

        private static string connectionString = string.Empty;

        /// <summary> 
        /// 尝试打开SQLite连接
        /// </summary> 
        /// <param name="filePath">SQLite数据库文件路径</param>
        public static void OpenSQLite(string filePath)
        {
            connection = new SQLiteConnection("Data Source=" + filePath);

            try { connection.Open(); }
            catch (Exception) { throw; }
        }

        /// <summary> 
        /// 执行一个查询语句，返回一个包含查询结果的DataTable 
        /// </summary> 
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string tableName)
        {
            DataTable dataTable = new DataTable(tableName);
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter($"select * from {tableName};", connection);
            try
            {
                dataAdapter.FillSchema(dataTable, SchemaType.Source);
                dataAdapter.Fill(dataTable);
            }
            catch (Exception) { throw; }
            finally { dataAdapter.Dispose(); }

            dataTable.AcceptChanges();
            return dataTable;
        }

        /// <summary>
        /// 如果表不存在则创建一个新表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static void CreatDataTable(string tableName, object[] parameters)
        {
            StringBuilder builder = new StringBuilder();
            if (parameters != null)
            {
                foreach (var item in parameters) builder.Append(item).Append(", ");
                builder.Remove(builder.Length - 2, 2);
            }
            string cmd = $"create table if not exists '{tableName}'({builder.ToString()});";
            SQLiteCommand command = new SQLiteCommand(cmd, connection);
            try { command.ExecuteNonQuery(); }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// 插入和更新datatable所有数据到数据库中
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static void SaveDataTable(DataTable dataTable, string tableName)
        {
            //string command = $"delete from '{tableName}';";
            try
            {
                //SQLiteCommand cmdCreateTable = new SQLiteCommand(command, m_dbConnection);
                //cmdCreateTable.ExecuteNonQuery();

                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter($"select * from {tableName};", connection);
                SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(dataAdapter);

                dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
                dataAdapter.Update(dataTable);

                dataTable.AcceptChanges();
                dataAdapter.Dispose();
            }
            catch (Exception e)
            {
                e.ToString();
                throw;
            }
        }

        /// <summary>
        /// 更新datatable所有数据到数据库中
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static void CommonAction(string cmd, params SQLiteParameter[] par)
        {
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                SQLiteCommand command = new SQLiteCommand(connection);

                command.Transaction = transaction;
                command.CommandText = cmd;
                if (par != null) command.Parameters.AddRange(par);

                command.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        /// <summary>
        /// 使用事务将DataTable的操作同步到数据库
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="command"></param>
        public static void UpdateTableAction(this DataTable dataTable)
        {
            SQLiteTransaction transaction = connection.BeginTransaction();

            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter($"select * from {dataTable};", connection);
            SQLiteCommandBuilder commandBuilder = new SQLiteCommandBuilder(dataAdapter);

            dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
            dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
            dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();

            try { dataAdapter.Update(dataTable); }
            catch (Exception) { throw; }
            finally { dataAdapter.Dispose(); }
            dataTable.AcceptChanges();

            transaction.Commit();
        }
    }
}
