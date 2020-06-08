using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using IC.Core;

namespace IC.WCF
{
    /// <summary>
    /// WCF 服务 [单利模式] [允许并发]
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ICWcfService : ICServer, IICWcfService
    {
        public void RegisterClient(string clientId)
        {
            if (this.Clients.ContainsKey(clientId))
            {
                throw new Exception("you have register client with!");
            }

            ICWcfConnection connection = new ICWcfConnection(OperationContext.Current);
            
            OperationContext.Current.Channel.Closed += (s, e) =>
            {
                this.RemoveClient(clientId);
            };

            base.RegisterClient(new Client(connection, clientId));
        }
        
        public MessageResponse SendMessage(MessageRequest messageRequest)
        {
            ICommandProcessor commandProcessor = null;
            var requestCommand = messageRequest.GetRequestCommand();
            commandProcessor.Initlize(null);
            return new MessageResponse()
            {
                CommandResponseJson = commandProcessor.InternalProcess(requestCommand).ToJson(),
                MessageGuid = messageRequest.MessageGuid,
                MessageContentType = messageRequest.MessageType
            };
        }
    }
}
