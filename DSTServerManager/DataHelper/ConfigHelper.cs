using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Data;
using System.Xml;

namespace DSTServerManager.DataHelper
{
    class ConfigHelper
    {
        /// <summary>   
        /// 将配置写入文件   
        /// </summary>   
        /// <param name="key"></param>   
        /// <param name="value"></param>   
        public static void SetValue(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null) config.AppSettings.Settings.Add(key, value);
            else config.AppSettings.Settings[key].Value = value;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>   
        /// 读取指定Key的值
        /// </summary>   
        /// <param name="key"></param>   
        /// <returns></returns>   
        public static string GetValue(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null)
                return "";
            else
                return config.AppSettings.Settings[key].Value;
        }

        /// <summary>
        /// 读取指定Key的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetIntValue(string key)
        {
            int keyValue = -1;
            int.TryParse(GetValue(key), out keyValue);
            return keyValue;
        }

        /// <summary>
        /// 读取指定Key的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetBoolValue(string key)
        {
            bool keyValue = false;
            bool.TryParse(GetValue(key), out keyValue);
            return keyValue;
        }

        /// <summary>
        /// 读取指定Key的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string[] GetGroupValue(string key)
        {
            string[] keyValue = GetValue(key).Split(',');
            return keyValue;
        }

        /// <summary>
        /// 保存一个数据表文件
        /// </summary>
        /// <param name="table"></param>
        public static void SetTable(DataTable table)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            XmlTextWriter writer = new XmlTextWriter(config.FilePath, Encoding.GetEncoding("UTF-8"));
            //writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;
            writer.Formatting = Formatting.Indented;

            writer.WriteStartDocument();
            writer.WriteComment("DataTable: " + table.TableName);
            writer.WriteStartElement("DataTable"); //DataTable开始

            writer.WriteAttributeString("TableName", table.TableName);
            writer.WriteAttributeString("CountOfRows", table.Rows.Count.ToString());
            writer.WriteAttributeString("CountOfColumns", table.Columns.Count.ToString());

            writer.WriteStartElement("ClomunName", "");
            for (int i = 0; i < table.Columns.Count; i++)
                writer.WriteAttributeString("Column" + i.ToString(), table.Columns[i].ColumnName);
            writer.WriteEndElement();

            for (int j = 0; j < table.Rows.Count; j++)
            {
                writer.WriteStartElement("Row" + j.ToString(), "");
                for (int k = 0; k < table.Columns.Count; k++)
                    writer.WriteAttributeString("Column" + k.ToString(), table.Rows[j][k].ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); //DataTable结束

            writer.WriteEndDocument();
            writer.Close();
        }


        public static DataTable GetTable()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            DataTable table = new DataTable();
            if (!File.Exists(config.FilePath)) return table;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(config.FilePath);

            XmlNode root = xmlDoc.SelectSingleNode("DataTable");

            table.TableName = ((XmlElement)root).GetAttribute("TableName");
            int CountOfRows = 0;
            int.TryParse(((XmlElement)root).GetAttribute("CountOfRows").ToString(), out CountOfRows);
            int CountOfColumns = 0;
            int.TryParse(((XmlElement)root).GetAttribute("CountOfColumns").ToString(), out CountOfColumns);

            foreach (XmlAttribute xa in root.ChildNodes[0].Attributes)
                table.Columns.Add(xa.Value);

            for (int i = 1; i < root.ChildNodes.Count; i++)
            {
                string[] array = new string[root.ChildNodes[0].Attributes.Count];
                for (int j = 0; j < array.Length; j++)
                    array[j] = root.ChildNodes[i].Attributes[j].Value.ToString();
                table.Rows.Add(array);
            }
            return table;
        }
    }
}
