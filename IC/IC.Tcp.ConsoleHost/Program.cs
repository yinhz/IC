using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IC.Tcp.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            IC.Tcp.ICTcpServer server = new ICTcpServer();
            
            server.OnClientConnected += (s, e) =>
            {
                // 记录到数据库，记录 ClientId 和 机器信息
                Console.WriteLine("On Client Connected : " + e.Client.ClientId);
                Console.WriteLine("On Client Connected OperationContext : " + (e.Client.Connection as ICTcpConnection).SessionId);
            };
            server.OnClientDisconnected += (s, e) =>
            {
                // 移除数据库记录， 记录的 ClientId 和 机器信息
                Console.WriteLine("On Client Disconnected : " + e.Client.ClientId);
                Console.WriteLine("On Client Disconnected. OperationContext : " + (e.Client.Connection as ICTcpConnection).SessionId);
            };
            server.OnMessageRequestReceived += (s, e) =>
            {
                // 可以记录到数据库
                Console.WriteLine("On Client Message Received. Command : " + e.MessageRequest.CommandRequestJson);
            };
            server.OnMessageResponseReceived += (s, e) =>
            {
                // 可以记录到数据库
                Console.WriteLine("On Client Message Response. Command : " + e.MessageResponse.CommandResponseJson);
            };

            server.Start("127.0.0.1", 36001);

            while (true)
            {
                Console.ReadKey();
            }
        }
    }
}
