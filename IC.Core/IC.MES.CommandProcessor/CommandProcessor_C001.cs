using IC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IC.WCF.InternalCommand
{
    [CommandProcessorDescription("C001")]
    public class CommandProcessor_C001 : CommandProcessor<RequestCommand_001, ResponseCommand>
    {
        public override ResponseCommand Process(RequestCommand_001 requestCommand)
        {
            return new ResponseCommand() { IsSuccess = true };
        }
    }

    public class RequestCommand_001 : RequestCommand
    {
        public string EquipmentCode { get; set; }
    }
}
