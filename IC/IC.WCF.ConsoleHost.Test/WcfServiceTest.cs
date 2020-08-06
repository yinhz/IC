using IC.Core;
using IC.WCF.Client;
using IC.WCF.Client.ICWcfService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IC.WCF.ConsoleHost.Test
{
    [TestClass]
    public class WcfServiceTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            ICClient client = new ICWcfClient(
                System.Guid.NewGuid().ToString(),
                (messageRequest) =>
                {
                    System.Diagnostics.Debug.WriteLine("Received from server. MessageRequest : " + Newtonsoft.Json.JsonConvert.SerializeObject(messageRequest));
                    return new MessageResponse()
                    {
                        MessageGuid = messageRequest.MessageGuid,
                        CommandResponseJson = "I am response for server request. MessageGuid : " + messageRequest.MessageGuid
                    };
                }
            );

            (client as ICWcfClient).TestWSBinding();

            client.SendMessage(new MessageRequest()
            {
                CommandId = "C001",
                MessageGuid = System.Guid.NewGuid(),
                RequestDate = DateTime.Now,
                CommandRequestJson = "{\"EquipmentCode\":\"" + "000" + "\"}"
            });

            int index = 0;

            while (true)
            {
                System.Threading.Tasks.Parallel.For(0, 5, (i) =>
                {
                    index = System.Threading.Interlocked.Add(ref index, 1);

                    try
                    {
                        client.SendMessage(new MessageRequest()
                        {
                            CommandId = "C001",
                            MessageGuid = System.Guid.NewGuid(),
                            RequestDate = DateTime.Now,
                            CommandRequestJson = "{\"EquipmentCode\":\"" + index.ToString() + "\"}"
                        });
                    }
                    catch
                    {
                    }
                });

                System.Threading.Thread.Sleep(1000);
            }

            client.Close();
        }
    }
}
