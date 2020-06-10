using IC.Core;
using System;
using System.ServiceModel;

namespace IC.WCF
{
    public class ICWcfConnection : IConnection
    {
        public OperationContext OperationContext { get; private set; }

        protected IICWcfCallBackService callBackService;

        public ICWcfConnection(OperationContext operationContext)
        {
            if (operationContext == null)
                throw new ArgumentNullException("operationContext");

            this.OperationContext = operationContext;
            callBackService = OperationContext.GetCallbackChannel<IICWcfCallBackService>();
        }

        public string SessionId => OperationContext.SessionId;

        public void Close()
        {
            OperationContext?.Channel?.Close();
            OperationContext = null;
            callBackService = null;
        }

        public void Dispose()
        {
            this.Close();
        }

        public MessageResponse SendMessageToClient(MessageRequest messageRequest)
        {
            if (this.callBackService == null)
            {
                throw new Exception("Can not get callback servie!");
            }

            return this.callBackService.SendMessageToClient(messageRequest);
        }
    }
}
