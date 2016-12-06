using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DSTServerManager.DataHelper
{
    static class ExtendHelper
    {
        /// <summary>
        /// [重要]拷贝类实例中与目标类实例相同的所有属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyAllProperties<T>(this object source, T target) where T : class
        {
            if (source == null || target == null)
                return;

            var properties = target.GetType().GetProperties();
            foreach (var targetPro in properties)
            {
                try
                {
                    //判断源对象是否存在与目标属性名字对应的源属性  
                    if (source.GetType().GetProperty(targetPro.Name) == null)
                    {
                        continue;
                    }
                    //数据类型不相等  
                    if (targetPro.PropertyType.FullName != source.GetType().GetProperty(targetPro.Name).PropertyType.FullName)
                    {
                        continue;
                    }
                    var propertyValue = source.GetType().GetProperty(targetPro.Name).GetValue(source, null);
                    if (propertyValue != null)
                    {
                        target.GetType().InvokeMember(targetPro.Name, BindingFlags.SetProperty, null, target, new object[] { propertyValue });
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// [重要]合并一个具有相同结构的Excel表的数据
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="excelData"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static void MergeExcelData(this DataTable dataTable, ExcelHelper excelData, string tableName)
        {
            DataTable localServerList = excelData.ExecuteDataTable(tableName);
            for (int i = 0; i < localServerList.Rows.Count; i++)
                dataTable.Rows.Add(localServerList.Rows[i].ItemArray);

            for (int i = 0; i < dataTable.Rows.Count; i++)
                dataTable.Rows[i][0] = 0;

            DataTable temp = dataTable.DefaultView.ToTable(true).Copy();
            dataTable.Clear();

            for (int i = 0; i < temp.Rows.Count; i++)
                dataTable.Rows.Add(temp.Rows[i].ItemArray);

            for (int i = 0; i < dataTable.Rows.Count; i++)
                dataTable.Rows[i][0] = i + 1;
        }

        /// <summary>
        /// 合并和重新编号datatable
        /// </summary>
        /// <param name="dataTable"></param>
        public static void RefreshDataTable(this DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
                dataTable.Rows[i][0] = 0;

            DataTable temp = dataTable.DefaultView.ToTable(true).Copy();
            dataTable.Clear();

            for (int i = 0; i < temp.Rows.Count; i++)
                dataTable.Rows.Add(temp.Rows[i].ItemArray);

            for (int i = 0; i < dataTable.Rows.Count; i++)
                dataTable.Rows[i][0] = i + 1;
        }

        public static DataTable CopyConfirmTable(this DataTable dataTable, int column, List<string> item)
        {
            DataTable current = dataTable.Clone();
            foreach (DataRow row in dataTable.Rows)
                if (item.Contains(row[column].ToString())) current.Rows.Add(row.ItemArray);
            current.RefreshDataTable();
            return current.Copy();
        }
    }
}
