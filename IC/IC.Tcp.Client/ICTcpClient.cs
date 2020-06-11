using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IC.Tcp.Client
{
    public class ICTcpClient 
    {
        private TcpClient tcpClient;
        public ICTcpClient(string server = "127.0.0.1", int port = 36001)
        {
            this.tcpClient = new System.Net.Sockets.TcpClient();
            tcpClient.Connect(server, port);
        }
        
        public void Connect()
        {
            //using ()
            //{

                

            //    MessageRequest messageRequest = new MessageRequest() { CommandId = "C001", CommandRequestJson = "{\"ABC\":\"123\"}" };

            //    byte[] messageBytes = messageRequest.SerializeToBinaryFormatter();

            //    byte[] data = new byte[IC_TCP_MESSAGE_HEADER.HeaderLength + messageBytes.Length];

            //    IC_TCP_MESSAGE_HEADER header = IC_TCP_MESSAGE_HEADER.CreateICTcpMessageHeader(MessageType.Request, messageBytes.Length);

            //    Buffer.BlockCopy(Utils.StructToBytes(header), 0, data, 0, IC_TCP_MESSAGE_HEADER.HeaderLength - 1);
            //    Buffer.BlockCopy(messageBytes, 0, data, IC_TCP_MESSAGE_HEADER.HeaderLength, messageBytes.Length - 1);

            //    while (true)
            //    {
            //        var ns = tcpClient.GetStream();
            //        ns.Write(data, 0, data.Length);
            //        System.Threading.Thread.Sleep(100);
            //    }
            //}
        }
    }
}