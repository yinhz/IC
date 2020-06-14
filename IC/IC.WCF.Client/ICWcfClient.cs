using IC.Core;
using IC.WCF.Client.ICWcfService;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace IC.WCF.Client
{
    public class ICWcfClient : ICClient
    {
        private _ICWcfServiceClient client;

        public ICWcfClient(string clientId, MessageReceivedDelegate messageReceived)
            : base(clientId, messageReceived)
        {
        }

        protected override void Reset()
        {
            try
            {
                if (this.client != null)
                {
                    this.client.Abort();
                }
            }
            finally
            {
                this.client = null;
            }

            this.client = new _ICWcfServiceClient(new InstanceContext(new ICWcfClientCallback(this.MessageReceived)));
        }

        internal class ICWcfClientCallback : _ICWcfServiceCallback
        {
            private MessageReceivedDelegate messageReceived;
            internal ICWcfClientCallback(MessageReceivedDelegate messageReceived)
            {
                if (messageReceived == null)
                    throw new ArgumentNullException("messageReceived");

                this.messageReceived = messageReceived;
            }
            public MessageResponse SendMessageToClient(MessageRequest messageRequest)
            {
                MessageResponse messageResponse = new MessageResponse()
                {
                    MessageGuid = messageRequest.MessageGuid,
                    ResponseDate = DateTime.UtcNow
                };

                if (messageReceived == null)
                    return messageResponse;

                return messageReceived.Invoke(messageRequest);
            }
        }

        protected override void DoRegister()
        {
            this.client.RegisterClient(this.clientId);
        }

        protected override MessageResponse DoSendMessage(MessageRequest messageRequest)
        {
            return client.SendMessage(messageRequest);
        }

        protected override Task<MessageResponse> DoSendMessageAsync(MessageRequest messageRequest)
        {
            return client.SendMessageAsync(messageRequest);
        }

        public override void Close()
        {
            try
            {
                if (this.client != null && this.client.State != System.ServiceModel.CommunicationState.Closed)
                    this.client?.Close();
            }
            catch
            {
                this.client?.Abort();
            }
        }

        public override void Dispose()
        {
            this.Close();
        }
        
        protected override void DoOpen()
        {
            client.Open();
        }
    }
}
