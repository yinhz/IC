using System;
using System.Threading.Tasks;

namespace IC.Core
{
    public delegate MessageResponse MessageReceivedDelegate(MessageRequest messageRequest);

    public abstract class ICClient
    {
        public ICClient(string clientId, MessageReceivedDelegate messageReceived)
        {
            if (messageReceived == null)
                throw new ArgumentNullException("messageReceived");

            this.CommunicationState = CommunicationState.Created;
            this.MessageReceived = messageReceived;
            this.clientId = clientId;
        }

        protected string clientId;
        public bool Registered { get; protected set; } = false;

        private object opening_lock = new object();

        private CommunicationState communicationState;
        public CommunicationState CommunicationState
        {
            get
            {
                return communicationState;
            }
            protected set
            {
                communicationState = value;
                StateChanged?.Invoke(communicationState, null);
            }
        }
        public MessageReceivedDelegate MessageReceived { get; private set; }

        public event EventHandler OnOpening;
        public event EventHandler OnOpenFailed;
        public event EventHandler StateChanged;

        /// <summary>
        /// Reset will be called when open failed
        /// </summary>
        protected abstract void Reset();

        protected virtual void Open()
        {
            this.Reset();
            this.DoOpen();
            this.Register();
        }
        protected abstract void DoOpen();

        public abstract void Dispose();

        public virtual void Register()
        {
            this.DoRegister();
            this.Registered = true;
        }

        private void OperationEnsure()
        {
            EnsureConnection();

            EnsureRegistered();
        }

        private void EnsureConnection()
        {
            if (this.CommunicationState == CommunicationState.Opened)
                return;

            // 避免重复 open
            lock (this.opening_lock)
            {
                while (true)
                {
                    // 并发调用发送时，可能有多个线程在打开，锁定解除后可能已经打开，因此在判断一次
                    if (this.CommunicationState == CommunicationState.Opened)
                        break;

                    OnOpening?.Invoke(this, new EventArgs());

                    try
                    {
                        this.CommunicationState = CommunicationState.Opening;
                        this.Open();
                        this.CommunicationState = CommunicationState.Opened;
                        break;
                    }
                    catch (Exception e)
                    {
                        this.CommunicationState = CommunicationState.Closing;
                        this.Close();
                        this.CommunicationState = CommunicationState.Closed;

                        OnOpenFailed?.Invoke(e, null);

                        this.Reset();

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

        public virtual MessageResponse SendMessage(MessageRequest messageRequest)
        {
            do
            {
                this.OperationEnsure();
                try
                {
                    return this.DoSendMessage(messageRequest);
                }
                catch (Exception e)
                {
                    this.CommunicationState = CommunicationState.Faulted;
                }
                System.Threading.Thread.Sleep(500);
            }
            while (true);
        }
        public virtual Task<MessageResponse> SendMessageAsync(MessageRequest messageRequest)
        {
            do
            {
                this.OperationEnsure();
                try
                {
                    return this.DoSendMessageAsync(messageRequest);
                }
                catch (Exception e)
                {
                    this.CommunicationState = CommunicationState.Faulted;
                }
                System.Threading.Thread.Sleep(500);
            }
            while (true);
        }

        public abstract void Close();

        protected abstract MessageResponse DoSendMessage(MessageRequest messageRequest);

        protected abstract Task<MessageResponse> DoSendMessageAsync(MessageRequest messageRequest);

        protected abstract void DoRegister();
    }
}
