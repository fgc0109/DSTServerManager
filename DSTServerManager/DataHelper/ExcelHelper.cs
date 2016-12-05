using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Diagnostics;

namespace DSTServerManager.DataHelper
{
    class ExcelHelper
    {
        private OleDbConnection m_dbConnection = null;

        public OleDbConnection DBConnection
        { get { return m_dbConnection; } }

        /// <summary>
        /// 尝试打开Excel连接
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="engines">Excel对象接口引擎</param>
        /// <param name="version">Excel对象文件版本</param>
        /// <returns>文件打开状态</returns>
        public void OpenExcel(string filePath, ExcelEngines engines, ExcelVersion version)
        {
            StringBuilder connectBuilder = new StringBuilder();

            if (engines == ExcelEngines.JET)
                connectBuilder.Append("Provider=Microsoft.Jet.OleDb.4.0;");
            if (engines == ExcelEngines.ACE)
                connectBuilder.Append("Provider=Microsoft.Ace.OleDb.12.0;");

            connectBuilder.Append($"Data Source={filePath};");

            if (version == ExcelVersion.Office1997 || version == ExcelVersion.Office2000 || version == ExcelVersion.Office2002 || version == ExcelVersion.Office2003)
                connectBuilder.Append($"Extended Properties='Excel 8.0; HDR=Yes; IMEX=1;'");
            if (version == ExcelVersion.Office2007)
                connectBuilder.Append($"Extended Properties='Excel 12.0; HDR=Yes; IMEX=1;'");

            m_dbConnection = new OleDbConnection(connectBuilder.ToString());
            try { m_dbConnection.Open(); }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// 从指定的Excel获取用户存储表格
        /// </summary>
        /// <param name="excelConn"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string tableName)
        {
            DataTable dataTable = new DataTable(tableName); 
            try
            {
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter($"select * from [{tableName}$];", m_dbConnection);
                dataAdapter.Fill(dataTable);
            }
            catch (Exception) { throw; }

            return dataTable;
        }
    }

    /// <summary>
    /// Excel对象引擎
    /// </summary>
    public enum ExcelEngines
    {
        JET = 4,
        ACE = 12,
    }

    /// <summary>
    /// Excel对象版本
    /// </summary>
    public enum ExcelVersion
    {
        Office1997 = 8,
        Office2000 = 9,
        Office2002 = 10,
        Office2003 = 11,
        Office2007 = 12,
    }
}
