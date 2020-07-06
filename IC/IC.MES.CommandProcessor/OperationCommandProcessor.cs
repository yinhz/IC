using IC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IC.MES.CommandProcessor
{
    public class OperationCommandProcess : ICommandProcessor
    {
        public bool Processed { get; set; }

        public string InternalProcess(string requestCommandJson)
        {
            string responseCommandJson = this.CallOperation(requestCommandJson);
            this.Processed = true;
            return responseCommandJson;
        }

        public virtual string OperationCode => "MI.Request";
        public virtual string CallOperation(string requestCommandJson)
        {
            // 调用 operation, 固定传入 RequestCommandJson, Operation 固定返回 约定的 ResponseCommandJson 格式 json.
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
    }
}
