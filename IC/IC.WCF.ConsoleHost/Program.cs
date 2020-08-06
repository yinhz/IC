using IC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IC.WCF.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ICWcfService wcfService = Singleton<ICWcfService>.Instance;

            wcfService.LoadCommandProcessor(new Assembly[1] { Assembly.Load("IC.MES.CommandProcessor") });

            using (ServiceHost serviceHost = new ServiceHost(wcfService))
            {
                serviceHost.Open();
                wcfService.OnClientConnected += (s, e) =>
                {
                    // 记录到数据库，记录 ClientId 和 机器信息
                    Log("On Client Connected : " + e.Client.ClientId);
                    Log("On Client Connected OperationContext : " + (e.Client.Connection as ICWcfConnection).OperationContext.SessionId);
                };
                wcfService.OnClientDisconnected += (s, e) =>
                {
                    // 移除数据库记录， 记录的 ClientId 和 机器信息
                    Log("On Client Disconnected : " + e.Client.ClientId);
                    Log("On Client Disconnected. OperationContext : " + (e.Client.Connection as ICWcfConnection).OperationContext.SessionId);
                };
                wcfService.OnMessageRequestReceived += (s, e) =>
                {
                    // 可以记录到数据库
                    Log("On Client Message Received. Command : " + e.MessageRequest.CommandRequestJson);
                };
                wcfService.OnMessageResponseReceived += (s, e) =>
                {
                    // 可以记录到数据库
                    Log("On Client Message Response. Command : " + e.MessageResponse.CommandResponseJson);
                };

                System.Timers.Timer timer = new System.Timers.Timer(1000);
                timer.Elapsed += (s, e) =>
                {
                    timer.Stop();
                    Console.WriteLine(" Try send data to client!");
                    Console.WriteLine(" Find " + wcfService.Clients.Count + " Clients!");

                    foreach (var client in wcfService.Clients)
                    {
                        try
                        {
                            wcfService.SendMessageToClient(
                                client.Key,
                                new Core.MessageRequest()
                                { CommandId = System.Guid.NewGuid().ToString() });
                        }
                        catch (Exception e1)
                        {
                            Console.WriteLine("Send Message To Client Exception. " + e1.Message);
                        }
                    }
                    timer.Start();
                };
                timer.Start();

                while (true && Console.ReadLine() != "exit")
                {

                }
            }
        }

        static void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
