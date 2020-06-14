using System;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IC.Core;
using IC.Tcp.Client;

namespace IC.Tcp.ConsoleHost.Test
{
    [TestClass]
    public class TcpTest
    {
        [TestMethod]
        public void SendDataTest()
        {
            ICTcpClient icTcpClient = new ICTcpClient(
                System.Guid.NewGuid().ToString(),
                new IPEndPoint(IPAddress.Parse("127.0.0.1"), 36001),
                (messageRequest) =>
                {
                    return null;
                });

            while (true)
            {
                try
                {
                    var ns = icTcpClient.SendMessage(new MessageRequest()
                    {
                        MessageGuid = System.Guid.NewGuid(),
                        CommandRequestJson = "{\"ABC\":\"" + System.Guid.NewGuid() + "\"}"
                    });

                    System.Threading.Thread.Sleep(50);
                }
                catch (Exception e)
                {
                }
            }
            icTcpClient.Close();
        }
    }
}
