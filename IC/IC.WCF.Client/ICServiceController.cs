using IC.WCF.Client.ICWcfService;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace IC.WCF.Client
{
    public delegate MessageResponse MessageReceivedDelegate(MessageRequest messageRequest);

    public class ICServiceController : IDisposable
    {
        private object opening_lock = new object();

        public event EventHandler OnOpening;
        public event EventHandler OnOpenfailed;

        private bool Registered { get; set; } = false;

        static ICServiceController()
        {
            ICServiceInstance = new ICServiceController();
        }

        public static ICServiceController ICServiceInstance;

        public MessageReceivedDelegate MessageReceived;

        internal class WcfServiceCallback : _ICWcfServiceCallback
        {
            internal MessageReceivedDelegate MessageReceived;
            internal WcfServiceCallback(MessageReceivedDelegate messageReceived)
            {
                this.MessageReceived = messageReceived;
            }
            public MessageResponse SendMessageToClient(MessageRequest messageRequest)
            {
                MessageResponse messageResponse = new MessageResponse()
                {
                    MessageGuid = messageRequest.MessageGuid,
                    ResponseDate = DateTime.UtcNow
                };

                if (MessageReceived == null)
                    return messageResponse;

                return MessageReceived.Invoke(messageRequest);
            }
        }

        private _ICWcfServiceClient client;

        public CommunicationState State => client.InnerChannel.State;

        private ICServiceController()
        {
            client = new _ICWcfServiceClient(new System.ServiceModel.InstanceContext(new WcfServiceCallback(MessageReceived)));
        }

        private void EnsureConnection()
        {
            if (this.client?.State == CommunicationState.Opened)
                return;

            // 避免重复 open
            lock (this.opening_lock)
            {
                while (true)
                {
                    // 并发调用发送时，可能有多个线程在打开，锁定解除后可能已经打开，因此在判断一次
                    if (this.client?.State == CommunicationState.Opened)
                        break;

                    OnOpening?.Invoke(this, new EventArgs());

                    try
                    {
                        if (this.client == null || this.client.State == CommunicationState.Closed)
                            this.client = new _ICWcfServiceClient(new System.ServiceModel.InstanceContext(new WcfServiceCallback(MessageReceived)));

                        this.client.Open();
                        break;
                    }
                    catch (Exception e)
                    {
                        if (this.client != null)
                            this.client.Abort();

                        this.client = null;

                        OnOpenfailed?.Invoke(e, null);

                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
        }

        private void EnsureRegistered()
        {
            if (!Registered)
                throw new Exception("You need register first!");
        }

        public void Register(string clientId)
        {
            EnsureConnection();
            this.client.RegisterClient(clientId);
            this.Registered = true;
        }

        public MessageResponse SendMessage(MessageRequest messageRequest)
        {
            OperationEnsure();

            return client.SendMessage(messageRequest);
        }

        public Task<MessageResponse> SendMessageAsync(MessageRequest messageRequest)
        {
            OperationEnsure();

            return client.SendMessageAsync(messageRequest);
        }

        private void OperationEnsure()
        {
            EnsureConnection();

            EnsureRegistered();
        }

        public void Close()
        {
            this.client?.Close();
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}
