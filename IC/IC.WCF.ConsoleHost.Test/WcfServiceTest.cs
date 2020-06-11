using IC.WCF.Client.ICWcfService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IC.WCF.ConsoleHost.Test
{
    public class WcfCallbackSerice : _ICWcfServiceCallback
    {
        public MessageResponse SendMessageToClient(MessageRequest messageRequest)
        {
            Console.WriteLine("Received from server. MessageRequest : " + Newtonsoft.Json.JsonConvert.SerializeObject(messageRequest));
            return new MessageResponse()
            {
                MessageGuid = messageRequest.MessageGuid,
                CommandResponseJson = "I an response for server request. MessageGuid : " + messageRequest.MessageGuid
            };
        }
    }
    [TestClass]
    public class WcfServiceTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            WcfCallbackSerice wcfCallbackSerice = new WcfCallbackSerice();

            Client.ICWcfService._ICWcfServiceClient client = new Client.ICWcfService._ICWcfServiceClient(new System.ServiceModel.InstanceContext(wcfCallbackSerice));
            client.RegisterClient(System.Guid.NewGuid().ToString());

            int index = 0;

            while (true)
            {
                System.Threading.Tasks.Parallel.For(0, 5, (i) =>
                {
                    index = System.Threading.Interlocked.Add(ref index, 1);

                    client.SendMessage(new MessageRequest()
                    {
                        CommandId = "C001",
                        MessageGuid = System.Guid.NewGuid(),
                        RequestDate = DateTime.Now,
                        CommandRequestJson = "{\"EquipmentCode\":\"" + index.ToString() + "\"}"
                    });
                });

                System.Threading.Thread.Sleep(1000);
            }

            client.Close();
        }
    }
}
