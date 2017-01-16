using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using LuaInterface;
using System.IO;
using System.Data;
using System.Collections;

namespace DSTServerManager.Servers
{
    class ServerModInfo
    {
        private string m_FilePath = string.Empty;
        private string m_WorkShop = string.Empty;
        private DataTable m_Configuration = new DataTable();

        public ServerModInfo(string path)
        {
            m_FilePath = path + @"\modinfo.lua";
            m_WorkShop = path.Split('\\')[path.Split('\\').Length - 1].Replace("workshop-", "");
            LuaGetModInfo();

            m_Configuration.Columns.Add("id");
            m_Configuration.Columns.Add("name");
            m_Configuration.Columns.Add("label");
            m_Configuration.Columns.Add("hover");
            m_Configuration.Columns.Add("options");
            m_Configuration.Columns.Add("default");
        }

        public string WorkShop { get { return m_WorkShop; } }

        #region 信息字段

        //与配置文件内字段名相同,不能随意更改
        private string name;
        private string description;
        private string author;
        private string version;
        private double api_version;
        private bool dst_compatible;
        private bool dont_starve_compatible;
        private bool reign_of_giants_compatible;
        private bool all_clients_require_mod;

        private LuaTable configuration_options;
        private ListDictionary configuration;
        #endregion

        #region 信息属性

        public string Name { get { return name; } }
        public string Description { get { return description; } }
        public string Author { get { return author; } }
        public string Version { get { return version; } }
        public double Api_version { get { return api_version; } }
        public bool Dst_compatible { get { return dst_compatible; } }
        public bool Dont_starve_compatible { get { return dont_starve_compatible; } }
        public bool Reign_of_giants_compatible { get { return reign_of_giants_compatible; } }
        public bool All_clients_require_mod { get { return all_clients_require_mod; } }

        public LuaTable Configuration_options { get { return configuration_options; } }
        public ListDictionary Configuration { get { return configuration; } }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void LuaGetModInfo()
        {

            Lua luaFile = new Lua();
            var interData = luaFile.DoFile(m_FilePath);

            //name = Encoding.UTF8.GetString(Encoding.Default.GetBytes((string)luaFile[nameof(name)]));
            //description = Encoding.UTF8.GetString(Encoding.Default.GetBytes((string)luaFile[nameof(description)]));
            //author = Encoding.UTF8.GetString(Encoding.Default.GetBytes((string)luaFile[nameof(author)]));
            //version = Encoding.UTF8.GetString(Encoding.Default.GetBytes((string)luaFile[nameof(version)]));
            name = (string)luaFile[nameof(name)];
            description = (string)luaFile[nameof(description)];
            author = (string)luaFile[nameof(author)];
            version = (string)luaFile[nameof(version)];

            api_version = (double)luaFile[nameof(api_version)];
            dst_compatible = (bool?)luaFile[nameof(dst_compatible)] ?? false;
            dont_starve_compatible = (bool?)luaFile[nameof(dont_starve_compatible)] ?? false;
            reign_of_giants_compatible = (bool?)luaFile[nameof(reign_of_giants_compatible)] ?? false;
            all_clients_require_mod = (bool?)luaFile[nameof(all_clients_require_mod)] ?? false;

            configuration_options = luaFile[nameof(configuration_options)] as LuaTable;
            if (configuration_options == null) return;

            configuration = luaFile.GetTableDict(configuration_options);
           
            foreach (DictionaryEntry de in configuration)
            {
                var table = luaFile.GetTableDict((LuaTable)de.Value);

                object[] array = new object[5];

                int index = 0;
                foreach (DictionaryEntry item in table)
                {
                    array[index] = item.Value;
                    index++;
                }
                //m_Configuration.Rows.Add(new object[] { 1, de });
            }

        }

        /// <summary>
        /// 获取模组信息数组
        /// </summary>
        /// <returns></returns>
        public object[] GetItemArray()
        {
            var array = new object[]
            {
                false,
                false,
                WorkShop,
                name,
                author,
                version,
                api_version,
                dst_compatible,
                dont_starve_compatible,
                reign_of_giants_compatible,
                all_clients_require_mod,
                //description.Replace("\n"," ").Replace("\r"," ")
            };
            return array;
        }
    }

}