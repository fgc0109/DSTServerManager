using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using LuaInterface;
using System.IO;

namespace DSTServerManager.Servers
{
    class ServerModInfo
    {
        private string m_FilePath = string.Empty;
        private string m_WorkShop = string.Empty;

        public ServerModInfo(string path)
        {
            m_FilePath = path + @"\modinfo.lua";
            m_WorkShop = path.Split('\\')[path.Split('\\').Length - 1].Replace("workshop-", "");
            LuaDoFile();
        }

        public string WorkShop { get { return m_WorkShop; } }

        #region 信息字段

        private string name;
        private string description;
        private string author;
        private string version;
        private double api_version;
        private bool dst_compatible;
        private bool dont_starve_compatible;
        private bool reign_of_giants_compatible;
        private bool all_clients_require_mod;

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

        #endregion

        public void LuaDoFile()
        {
            Lua luaFile = new Lua();
            var interData = luaFile.DoFile(m_FilePath);

            name = Encoding.GetEncoding("UTF-8").GetString(Encoding.Default.GetBytes((string)luaFile[nameof(name)]));
            description = Encoding.GetEncoding("UTF-8").GetString(Encoding.Default.GetBytes((string)luaFile[nameof(description)]));
            author = Encoding.GetEncoding("UTF-8").GetString(Encoding.Default.GetBytes((string)luaFile[nameof(author)]));
            version = Encoding.GetEncoding("UTF-8").GetString(Encoding.Default.GetBytes((string)luaFile[nameof(version)]));

            api_version = (double)luaFile[nameof(api_version)];
            dst_compatible = (bool?)luaFile[nameof(dst_compatible)] ?? false;
            dont_starve_compatible = (bool?)luaFile[nameof(dont_starve_compatible)] ?? false;
            reign_of_giants_compatible = (bool?)luaFile[nameof(reign_of_giants_compatible)] ?? false;
            all_clients_require_mod = (bool?)luaFile[nameof(all_clients_require_mod)] ?? false;
        }

        public object[] GetItemArray()
        {
            var array = new object[]
            {
                WorkShop,
                name,
                author,
                version,
                api_version,
                dst_compatible,
                dont_starve_compatible,
                reign_of_giants_compatible,
                all_clients_require_mod,
                description.Replace("\n"," ").Replace("\r"," ")
            };
            return array;
        }
    }
}
