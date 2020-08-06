using IC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IC.MES.CommandProcessor
{
    [CommandProcessorDescription("C003")]
    public class CommandProcessor_C003 : CommandProcessor<RequestCommand_001, ResponseCommand_001>
    {
        public CommandProcessor_C003()
        {
        }

        public override RequestCommand_001 ParseCommand(string requestJson)
        {
            throw new NotImplementedException();
        }

        public override ResponseCommand_001 Process(RequestCommand_001 request)
        {
            throw new NotImplementedException();
        }
    }
}
