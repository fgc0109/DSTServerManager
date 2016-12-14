using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTServerManager
{
    class ServerReceivedEventArgs : EventArgs
    {
        private readonly string m_ReceivedData;

        public ServerReceivedEventArgs(string receivedData)
        {
            m_ReceivedData = receivedData;
        }
        public string ReceivedData { get { return m_ReceivedData; } }
    }
}
