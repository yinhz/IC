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
            IC_TCP_MESSAGE_STRUCT messageHeader = default(IC_TCP_MESSAGE_STRUCT);
            MessageRequest messageRequest = new MessageRequest() { CommandId = "Cxcyadsfnqwerjalxcvasdfsadf8" };

            IC_TCP_MESSAGE_STRUCT header = IC_TCP_MESSAGE_STRUCT.CreateICTcpMessageStruct(MessageType.Request, int.MaxValue, MessageFormat.Binary);

            var bytes = Utils.StructToBytes( header);

            var x = Utils.BytesToStruct<IC_TCP_MESSAGE_STRUCT>(bytes);

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
