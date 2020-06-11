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

            MessageRequest messageRequest = new MessageRequest() { CommandId = "C008" };

            ICTcpMessageContent content = new ICTcpMessageContent(messageRequest);

            ICTcpMessageHeadStruct structObj = new ICTcpMessageHeadStruct()
            {
                Header = typeof(ICTcpMessageContent).FullName,
                DataLength = content.DataLength
            };

            var bytes = Utils.StructToBytes<ICTcpMessageHeadStruct>(structObj);

            var o1 = Utils.BytesToStruct<ICTcpMessageHeadStruct>(bytes);
        }
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ICTcpMessageHeadStruct
    {
        public string Header;
        public int DataLength;
    }

}
