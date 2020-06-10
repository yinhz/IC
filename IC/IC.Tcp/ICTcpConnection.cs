using IC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IC.Tcp
{
    public class ICTcpConnection : IConnection
    {
        public string SessionId => throw new NotImplementedException();

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public MessageResponse SendMessageToClient(MessageRequest messageRequest)
        {
            throw new NotImplementedException();
        }
    }
}
