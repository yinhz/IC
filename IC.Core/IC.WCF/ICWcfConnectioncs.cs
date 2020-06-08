using IC.Core;
using System;
using System.Runtime.Remoting.Channels;
using System.ServiceModel;
using System.Threading.Tasks;

namespace IC.WCF
{
    public class ICWcfConnection : IConnection
    {
        private OperationContext _operationContext;
        private IICWcfCallBackService callBackService => _operationContext.GetCallbackChannel<IICWcfCallBackService>();

        public ICWcfConnection(OperationContext operationContext)
        {
            this._operationContext = operationContext;
            this.ConnectionContext = new WcfConnectionContext();
        }

        public IConnectionContext ConnectionContext { get; private set; }

        public string SessionId => _operationContext.SessionId;

        public void Close()
        {
            _operationContext.Channel.Close();
        }

        public void Dispose()
        {
            this.Close();
        }

        public Task Send(MessageRequest messageRequest)
        {
            if (this.callBackService == null)
            {
                throw new Exception("Can not get callback servie!");
            }

            this.callBackService.SendMessage(messageRequest);

            return Task.CompletedTask;
        }
    }

    public class WcfConnectionContext : IConnectionContext
    {
        public string MasterEquipmentCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string EquipmentCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
