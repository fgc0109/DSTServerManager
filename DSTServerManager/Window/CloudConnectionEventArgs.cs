using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTServerManager
{
    /// <summary>
    /// 容纳参数传递事件的附加信息
    /// </summary>
    public class CloudConnectionEventArgs : EventArgs
    {
        private readonly DataRow m_DataRow;
        private readonly bool m_NewRow;

        public CloudConnectionEventArgs(DataRow dataRow, bool newRow)
        {
            m_NewRow = newRow;
            m_DataRow = dataRow;
        }
        public DataRow GetRow { get { return m_DataRow; } }
        public bool IsNewRow { get { return m_NewRow; } }
    }
}
