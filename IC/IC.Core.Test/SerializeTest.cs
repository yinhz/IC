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
            var data = new
            {
                EquipmentCode = "EQU001",
                Status = "Run",
                Text = "abc\r\n"
            };

            MessageRequest m1 = new MessageRequest()
            {
                CommandId = "C008",
                MessageGuid = System.Guid.NewGuid(),
                RequestDate = DateTime.Now,
                CommandRequestJson =
                data.ToJson()
            };

            var bs = m1.SerializeToBinaryFormatter();
            var json = m1.ToJson();
            var m2 = bs.DeserializeToObject<MessageRequest>();

            MessageResponse response = new MessageResponse()
            {
                CommandId = "C008",
                Success = true,
                ErrorCode = "",
                ErrorMessage = "",
                MessageGuid = System.Guid.NewGuid(),
                ResponseDate = DateTime.Now,
                CommandResponseJson = data.ToJson()
            };

            var j1 = response.ToJson();

            Assert.AreEqual(m1.MessageGuid, m2.MessageGuid);
        }
    }
}
