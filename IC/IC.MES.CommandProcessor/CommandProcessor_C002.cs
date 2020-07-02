using IC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IC.MES.CommandProcessor
{
    [CommandProcessorDescription("C002")]
    public class CommandProcessor_C002 : CommandProcessor<RequestCommand_002, ResponseCommand>
    {
        public CommandProcessor_C002()
        {
        }

        public override RequestCommand_002 ParseCommand(string commandJson)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<RequestCommand_002>(commandJson);
        }

        public override ResponseCommand Process(RequestCommand_002 requestCommand)
        {
            return new ResponseCommand() { IsSuccess = true };
        }
    }

    public class RequestCommand_002 : RequestCommand
    {
        public string EquipmentCode { get; set; }
        
        public override string CommandId => "C002";
    }
}
