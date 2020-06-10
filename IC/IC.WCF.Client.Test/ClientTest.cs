using System;
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
            ICServiceController.ICServiceInstance.MessageReceived
                = (messageRequest) =>
                {
                    return new MessageResponse()
                    {
                        MessageGuid = messageRequest.MessageGuid
                    };
                };

            ICServiceController.ICServiceInstance.OnOpening += ICServiceInstance_Opening;
            ICServiceController.ICServiceInstance.OnOpenfailed += ICServiceInstance_Openfailed;

            ICServiceController.ICServiceInstance.Register(System.Guid.NewGuid().ToString());

            while (true)
            {
                System.Threading.Tasks.Parallel.For
                    (0, 100, (i) =>
                   {
                       System.Diagnostics.Debug.WriteLine("Message request! " + DateTime.Now);

                       try
                       {
                           ICServiceController.ICServiceInstance.SendMessage(new ICWcfService.MessageRequest()
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
                   });

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
