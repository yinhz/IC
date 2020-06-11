using IC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IC.Tcp
{
    public class ICTcpConnection : IConnection
    {
        private TcpClient tcpClient;
        public ICTcpConnection(TcpClient tcpClient)
        {
            if (tcpClient == null)
            {
                throw new ArgumentNullException("tcpClientWrapper");
            }

            this.tcpClient = tcpClient;
        }

        public string SessionId => tcpClient.GetHashCode().ToString();

        public void Close()
        {
        }

        public void Dispose()
        {
        }

        public MessageResponse SendMessageToClient(MessageRequest messageRequest)
        {
            throw new NotImplementedException();
        }
    }
}
