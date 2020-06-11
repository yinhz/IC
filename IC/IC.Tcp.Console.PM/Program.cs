using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IC.Tcp.Console.PM
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

                    _ICWcfServiceClient[] wcfCallbackSerices = new _ICWcfServiceClient[clientCount];
                    for (int i = 0; i < clientCount; i++)
                    {
                        WcfCallbackSerice wcfCallbackSerice = new WcfCallbackSerice();

                        wcfCallbackSerices[i] = new ICWcfService._ICWcfServiceClient(new System.ServiceModel.InstanceContext(wcfCallbackSerice));
                        wcfCallbackSerices[i].RegisterClient(System.Guid.NewGuid().ToString());
                    }

                    System.Threading.Tasks.Parallel.For(0, messageCount, new ParallelOptions() { MaxDegreeOfParallelism = messageCount }, (j) =>
                    {
                        System.Threading.Tasks.Parallel.For(0, clientCount, (i) =>
                        {
                            wcfCallbackSerices[i].SendMessage(new MessageRequest()
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
                        wcfCallbackSerices[i].Close();
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
