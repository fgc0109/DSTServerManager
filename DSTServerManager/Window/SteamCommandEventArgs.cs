using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTServerManager
{
    /// <summary>
    /// 容纳参数传递事件的附加信息
    /// </summary>
    public class SteamCommandEventArgs : EventArgs
    {
        private readonly string m_NewServerPath;

        public SteamCommandEventArgs(string NewServerPath)
        {
            m_NewServerPath = NewServerPath;
        }
        public string NewServerPath { get { return m_NewServerPath; } }
    }
}
