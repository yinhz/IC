using IC.Tcp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IC.Core.Test
{
    [TestClass]
    public class SerializeTest
    {
        [TestMethod]
        public void TestMethod()
        {
            MessageRequest m1 = new MessageRequest() { CommandId = "C008" };

            var bs = m1.SerializeToBinaryFormatter();

            var m2 = bs.DeserializeToObject<MessageRequest>();

            Assert.AreEqual(m1.MessageGuid, m2.MessageGuid);
        }
    }
}
