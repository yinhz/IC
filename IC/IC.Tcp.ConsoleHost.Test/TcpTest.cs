using System;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IC.Core;

namespace IC.Tcp.ConsoleHost.Test
{
    [TestClass]
    public class TcpTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (System.Net.Sockets.TcpClient tcpClient = new System.Net.Sockets.TcpClient())
            {
                tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 36001);

                string str = "i am test message";

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);

                while (true)
                {
                    tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                    System.Threading.Thread.Sleep(100);
                    break;
                }

                tcpClient.Close();
            }
        }
    }
}
