using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class ICWcfService : ICServer, _ICWcfService
    {
        public ICWcfService()
            : base()
        {
        }

        public void LoadCommandProcessor(Assembly[] assemblies)
        {
            this.LoadCommandProcessors(assemblies);
        }

        public void RegisterClient(string clientId)
        {
            if (this.Clients.ContainsKey(clientId))
            {
                throw new Exception("you have register client with!");
            }

            ICWcfConnection connection = new ICWcfConnection(OperationContext.Current);

            OperationContext.Current.Channel.Closed += (s, e) =>
            {
                this.ClientDisonnected(clientId);
            };

            base.ClientConnect(new Client(clientId, connection));
        }

        public MessageResponse SendMessage(MessageRequest messageRequest)
        {
            // to-do 还要根据 session id 识别 client
            // client 信息是否有用？
            // 放什么信息？ equipment?
            return base.ClientMessageRequest(messageRequest);
        }
    }
}
