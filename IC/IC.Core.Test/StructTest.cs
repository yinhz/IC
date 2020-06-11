using System;
using System.Runtime.InteropServices;
using IC.Tcp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IC.Core.Test
{
    [TestClass]
    public class StructTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            IC_TCP_MESSAGE_HEADER messageHeader = default(IC_TCP_MESSAGE_HEADER);
            MessageRequest messageRequest = new MessageRequest() { CommandId = "Cxcyadsfnqwerjalxcvasdfsadf8" };

            IC_TCP_MESSAGE_HEADER header = IC_TCP_MESSAGE_HEADER.CreateICTcpMessageHeader(MessageType.Request, int.MaxValue);

            var bytes = Utils.StructToBytes( header);

            var x = Utils.BytesToStruct<IC_TCP_MESSAGE_HEADER>(bytes);

            //ICTcpHeaderStruct structObj = new ICTcpHeaderStruct()
            //{
            //    Header = typeof(ICTcpMessageContent).FullName,
            //    DataLength = content.DataLength
            //};

            //var bytes = Utils.StructToBytes<ICTcpHeaderStruct>(structObj);

            //var o1 = Utils.BytesToStruct<ICTcpHeaderStruct>(bytes);
        }
    }
}
