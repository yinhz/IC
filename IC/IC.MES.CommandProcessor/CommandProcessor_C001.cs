using IC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IC.MES.CommandProcessor
{
    [CommandProcessorDescription("C001")]
    public class CommandProcessor_C001 : CommandProcessor<RequestCommand_001, ResponseCommand_001>
    {
        public CommandProcessor_C001()
        {
        }

        public override RequestCommand ParseCommand(string commandJson)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<RequestCommand_001>(commandJson);
        }

        public override ResponseCommand_001 Process(RequestCommand_001 requestCommand, MessageRequest messageRequest)
        {
            return new ResponseCommand_001() { IsSuccess = true, Content = messageRequest.CommandRequestJson };
        }
    }

    public class RequestCommand_001 : RequestCommand
    {
        public string EquipmentCode { get; set; }

        public override string CommandId => "C001";
    }
    public class ResponseCommand_001 : ResponseCommand
    {
        public string Content { get; set; }
    }
}
