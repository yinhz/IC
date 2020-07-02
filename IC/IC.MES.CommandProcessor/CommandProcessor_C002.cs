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
    public class CommandProcessor_C002 : OperationCommandProcess
    {
        public CommandProcessor_C002()
        {
        }

        public override string OperationCode => base.OperationCode;
        public override string CallOperation(string requestCommandJson)
        {
            return base.CallOperation(requestCommandJson);
        }
    }
}
