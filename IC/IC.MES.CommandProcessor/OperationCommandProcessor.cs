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
    public class OperationCommandProcess : CommandProcessor<RequestCommand, ResponseCommand>
    {
        public override string InternalProcess(string requestCommandJson)
        {
            return base.InternalProcess(requestCommandJson);
        }
        public virtual string OperationCode => "MI.Request";
        public virtual string CallOperation(string requestCommandJson)
        {
            // 调用 operation, 固定传入 requestCommandJson, Operation 固定返回 约定的 responseCommand 格式 json.
            // 暂时不作验证？
            try
            {
                // call operation
                return requestCommandJson;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public override RequestCommand ParseCommand(string commandJson)
        {
            throw new NotImplementedException();
        }

        public override ResponseCommand Process(RequestCommand requestCommand)
        {
            throw new NotImplementedException();
        }
    }
}
