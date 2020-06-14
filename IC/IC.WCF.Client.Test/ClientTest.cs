using System;
using IC.Core;
using IC.WCF.Client.ICWcfService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IC.WCF.Client.Test
{
    [TestClass]
    public class ClientTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var client = new ICWcfClient(System.Guid.NewGuid().ToString(), (messageRequest) =>
             {
                 return new MessageResponse()
                 {
                     MessageGuid = messageRequest.MessageGuid
                 };
             });

            client.OnOpening += ICServiceInstance_Opening;
            client.OnOpenFailed += ICServiceInstance_Openfailed;
            
            while (true)
            {
                System.Diagnostics.Debug.WriteLine("Message request! " + DateTime.Now);

                try
                {
                    client.SendMessage(new MessageRequest()
                    {
                        MessageGuid = System.Guid.NewGuid(),
                        CommandId = "C001",
                        RequestDate = DateTime.Now,
                        CommandRequestJson = "{}"
                    });
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Send Message exception! " + e.Message);
                    System.Diagnostics.Debug.WriteLine("Send Message exception! " + e.StackTrace);
                }

                System.Threading.Thread.Sleep(500);
            }
        }

        private void ICServiceInstance_Openfailed(object sender, EventArgs e)
        {
            if (sender is Exception)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " . " + (sender as Exception).Message);
                System.Diagnostics.Debug.WriteLine(DateTime.Now + " . " + (sender as Exception).StackTrace);
            }
        }

        private void ICServiceInstance_Opening(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " . Opening!");
        }
    }
}
