using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections;

namespace DSTServerManager.DataHelper
{
    /// <summary>
    /// 读取INI数据流
    /// 避免文件编码问题 不能使用kernel32
    /// </summary>
    class IniHelper
    {
        public Dictionary<string, Dictionary<string, string>> m_Section = new Dictionary<string, Dictionary<string, string>>();

        public IniHelper(MemoryStream stream, bool readUnknowSectionKey)
        {
            AnalysisFile(stream, readUnknowSectionKey);
        }

        /// <summary>
        /// 获取INI数据
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaulted"></param>
        /// <returns></returns>
        public string ReadIniData(string section, string key, string defaulted)
        {
            if (m_Section.Count <= 0)
                return defaulted;
            if (m_Section.ContainsKey(section) && m_Section[section].ContainsKey(key))
                return m_Section[section][key].ToString();
            else
                return defaulted;
        }

        /// <summary>
        /// 写入INI数据
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteIniData(string section, string key, string value)
        {
            if (m_Section.ContainsKey(section) && m_Section[section].ContainsKey(key))
                m_Section[section][key] = value;
            else if (m_Section.ContainsKey(section) && !m_Section[section].ContainsKey(key))
            {
                m_Section[section].Add(key, value);
            }
            else if (!m_Section.ContainsKey(section))
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add(key, value);
                m_Section.Add(section, data);
            }
        }

        /// <summary>
        /// 分析文件数据
        /// </summary>
        /// <param name="stream"></param>
        public void AnalysisFile(MemoryStream stream, bool readUnknowSectionKey)
        {
            Dictionary<string, string> data = null;
            StreamReader reader = new StreamReader(stream, Encoding.Default);
            string line;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();

                //处理注释 移除;后的注释内容
                if (line.Contains(";"))
                {
                    string[] info_note = line.Split(';');
                    line = info_note[0];
                }
                //处理section行 读取或创建
                if (line.Contains("[") && line.Contains("]"))
                {
                    string section = line.Replace("[", "").Replace("]", "").Replace(" ", "");
                    if (m_Section.ContainsKey(section))
                    {
                        data = m_Section[section];
                    }
                    else
                    {
                        data = new Dictionary<string, string>();
                        m_Section.Add(section, data);
                    }
                    continue;
                }

                //处理key行 读取或创建
                if (line.Contains("="))
                {
                    if (readUnknowSectionKey)
                    {
                        string section = "UNKNOW";
                        if (null == data && m_Section.ContainsKey(section))
                        {
                            data = m_Section[section];
                        }
                        else if (null == data && !m_Section.ContainsKey(section))
                        {
                            data = new Dictionary<string, string>();
                            m_Section.Add(section, data);
                        }
                    }
                    else if (null == data)
                        continue;

                    string key = line.Split('=')[0].Replace(" ", "");
                    string value = line.Split('=')[1].Replace(" ", "");

                    if (data.ContainsKey(key)) data[key] = value;
                    else data.Add(key, value);
                    continue;
                }
            }
            reader.Close();
        }

        /// <summary>
        /// 获取字典结构对应的内存流
        /// </summary>
        public MemoryStream GetIniStream()
        {
            MemoryStream stream = new MemoryStream();

            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            IDictionaryEnumerator section = m_Section.GetEnumerator();
            writer.WriteLine();
            writer.WriteLine();
            while (section.MoveNext())
            {
                writer.WriteLine($"[{section.Key}]");
                IDictionaryEnumerator key = ((Dictionary<string, string>)(section.Value)).GetEnumerator();
                while (key.MoveNext())
                {
                    writer.WriteLine(key.Key + " = " + key.Value);
                }
                writer.WriteLine();
            }
            writer.Flush();
            writer.Close();
            return stream;
        }
    }
}