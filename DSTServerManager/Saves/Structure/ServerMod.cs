using System;
using LuaInterface;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DSTServerManager.Saves
{
    class ServerMod
    {
        private List<string> m_ServerMods = new List<string>();


        ///// <summary>
        ///// 从文件读取配置文件
        ///// </summary>
        ///// <param name="path">Mod配置文件路径</param>
        //public void ReadFromFile(string path)
        //{
        //    if (!File.Exists(path)) return;

        //    MemoryStream serverDataStream = new MemoryStream(File.ReadAllBytes(path));
        //    m_Setting = new IniHelper(serverDataStream, false);
        //    serverDataStream.Close();

        //    try { SettingToFields(); }
        //    catch (Exception) { throw; }
        //}

        ///// <summary>
        ///// 写入位于本地配置文件
        ///// </summary>
        ///// <param name="path">Mod配置文件路径</param>
        //public void WriteToFile(string path)
        //{
        //    if (null == m_Setting) return;
        //    try { FieldsToSetting(); }
        //    catch (Exception) { throw; }

        //    MemoryStream serverDataStream = m_Setting.GetIniStream();
        //    FileStream clusterFileStream = new FileStream(path, FileMode.Create);
        //    BinaryWriter w = new BinaryWriter(clusterFileStream);
        //    w.Write(serverDataStream.ToArray());
        //    clusterFileStream.Close();
        //    serverDataStream.Close();
        //}
    }
}
