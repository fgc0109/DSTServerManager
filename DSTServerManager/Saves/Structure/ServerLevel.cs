using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;
using System.Data;
using System.IO;

namespace DSTServerManager.Saves
{
    /// <summary>
    /// 获取存档设置信息
    /// </summary>
    class ServerLevel
    {
        object[] m_ServerLevelData = null;
        private readonly DataTable m_ServerDefault = new DefaultData().ServersLevelDefaultData();

        private DataTable m_ServerLevelTable = new DataTable();


        public DataTable ServerLevelTable
        {
            get { return m_ServerLevelTable; }
            set { m_ServerLevelTable = value; }
        }

        /// <summary>
        /// 从文件读取世界配置信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool ReadFromFile(string path)
        {
            Lua luaFile = new Lua();

            if (!File.Exists(path))
                return false;

            m_ServerLevelData = luaFile.DoFile(path);

            m_ServerLevelTable = new DefaultData().ServersLevelDefaultData();
            m_ServerLevelTable.Clear();

            Dictionary<object, object> serverLevel = luaFile.GetTableDict(m_ServerLevelData[0] as LuaTable);
            Dictionary<object, object> overrides = luaFile.GetTableDict(serverLevel["overrides"] as LuaTable);

            for (int i = 0; i < m_ServerDefault.Rows.Count; i++)
            {
                if (overrides.ContainsKey(m_ServerDefault.Rows[i][1]))
                {
                    DataRow current = m_ServerLevelTable.NewRow();

                    for (int j = 0; j < current.Table.Columns.Count; j++)
                    {
                        current[j] = m_ServerDefault.Rows[i][j];
                    }
                    current[7] = overrides[m_ServerDefault.Rows[i][1]];
                    m_ServerLevelTable.Rows.Add(current);
                }
            }
            return true;
        }
    }

    enum ServerLevelEnum
    {

    }
}
