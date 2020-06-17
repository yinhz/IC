using IC.Core;
using IC.Tcp.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IC.Tcp.ConsoleHost.PM
{
    class Program
    {
        static void Main(string[] args)
        {
            int clientCount = 0;
            int messageCount = 0;

            while (true)
            {
                try
                {
                    Console.WriteLine("Input client count.");
                    string strClientCount = Console.ReadLine();
                    clientCount = Convert.ToInt32(strClientCount);

                    Console.WriteLine("Input message count every client.");
                    string strMessageCount = Console.ReadLine();
                    messageCount = Convert.ToInt32(strMessageCount);

                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

                    Console.WriteLine("Test Begin." + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fffff"));

                    stopwatch.Start();

                    ICTcpClient[] clients = new ICTcpClient[clientCount];
                    for (int i = 0; i < clientCount; i++)
                    {
                        clients[i] = new ICTcpClient(System.Guid.NewGuid().ToString(), new System.Net.IPEndPoint(IPAddress.Parse("127.0.0.1"), 36001), (messageRequest) =>
                        {
                            return new MessageResponse()
                            {
                                MessageGuid = messageRequest.MessageGuid
                            };
                        });
                    }

                    System.Threading.Tasks.Parallel.For(0, messageCount, new ParallelOptions() { MaxDegreeOfParallelism = messageCount }, (j) =>
                    {
                        System.Threading.Tasks.Parallel.For(0, clientCount, (i) =>
                        {
                            try
                            {
                                clients[i].SendMessage(new MessageRequest()
                                {
                                    CommandId = "C001",
                                    MessageGuid = System.Guid.NewGuid(),
                                    RequestDate = DateTime.Now,
                                    CommandRequestJson = "{\"EquipmentCode\":\"" + System.Guid.NewGuid().ToString() + "    " + i.ToString() + "\"}"
                                });
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        });
                    });
                    for (int i = 0; i < clientCount; i++)
                    {
                        clients[i].Close();
                    }

                    stopwatch.Stop();

                    Console.WriteLine("Test End." + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fffff") + ".");
                    Console.WriteLine("Total ElapsedMilliseconds : " + stopwatch.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception!!!");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);

                    Console.WriteLine("Press any key to continue!");
                    Console.ReadKey();
                }
            }
        }
    }
}
