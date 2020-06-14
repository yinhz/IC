using IC.Core;
using IC.WCF.Client;
using IC.WCF.Client.ICWcfService;
using System;
using System.Threading.Tasks;

namespace IC.WCF.ConsoleHost.PM
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

                    ICClient[] wcfClients = new ICWcfClient[clientCount];
                    for (int i = 0; i < clientCount; i++)
                    {
                        wcfClients[i] = new ICWcfClient(
                            System.Guid.NewGuid().ToString(),
                            (messageRequest) =>
                            {
                                Console.WriteLine("Received from server. MessageRequest : " + messageRequest.MessageGuid);
                                return new MessageResponse()
                                {
                                    MessageGuid = messageRequest.MessageGuid,
                                    CommandResponseJson = "I an response for server request. MessageGuid : " + messageRequest.MessageGuid
                                };
                            }
                            );
                    }

                    System.Threading.Tasks.Parallel.For(0, messageCount, new ParallelOptions() { MaxDegreeOfParallelism = messageCount }, (j) =>
                    {
                        System.Threading.Tasks.Parallel.For(0, clientCount, (i) =>
                        {
                            wcfClients[i].SendMessage(new MessageRequest()
                            {
                                CommandId = "C001",
                                MessageGuid = System.Guid.NewGuid(),
                                RequestDate = DateTime.Now,
                                CommandRequestJson = "{\"EquipmentCode\":\"" + j.ToString() + "\"}"
                            });
                        });
                    });
                    for (int i = 0; i < clientCount; i++)
                    {
                        wcfClients[i].Close();
                    }

                    //Parallel.For(0, clientCount, new ParallelOptions() { MaxDegreeOfParallelism = clientCount }, (i) =>
                    //{
                    //    ICWcfClient client = new ICWcfClient(
                    //        (messageRequest) =>
                    //        {
                    //            Console.WriteLine("Received from server. MessageRequest : " + messageRequest.MessageGuid);
                    //            return new MessageResponse()
                    //            {
                    //                MessageGuid = messageRequest.MessageGuid,
                    //                CommandResponseJson = "I an response for server request. MessageGuid : " + messageRequest.MessageGuid
                    //            };
                    //        }
                    //        );
                    //    client.Register(System.Guid.NewGuid().ToString());

                    //    System.Threading.Tasks.Parallel.For(0, messageCount, new ParallelOptions() { MaxDegreeOfParallelism = messageCount }, (j) =>
                    //    {
                    //        client.SendMessage(new MessageRequest()
                    //        {
                    //            CommandId = "C001",
                    //            MessageGuid = System.Guid.NewGuid(),
                    //            RequestDate = DateTime.Now,
                    //            CommandRequestJson = "{\"EquipmentCode\":\"" + j.ToString() + "\"}"
                    //        });
                    //    });

                    //    client.Close();
                    //});

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
