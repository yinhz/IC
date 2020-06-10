using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace IC.Core
{
    /// <summary>
    /// 通信服务
    /// </summary>
    internal interface _ICServer
    {
        #region Client

        ClientConcurrentDictionary Clients { get; set; }
        void ClientConnect(IClient client);
        void ClientDisonnected(string clientId);

        #endregion

        #region Supported event

        event EventHandler<ClientConnectedEventArgs> OnClientConnect;
        event EventHandler<ClientConnectedEventArgs> OnClientConnected;
        event EventHandler<MessageRequestReceivedEventArgs> OnMessageRequestReceived;
        event EventHandler<MessageResponseReceivedEventArgs> OnMessageResponseReceived;
        event EventHandler<ClientDisonnectedEventArgs> OnClientDisconnect;
        event EventHandler<ClientDisonnectedEventArgs> OnClientDisconnected;

        #endregion

        #region Command processor

        ConcurrentDictionary<string, Type> CommandProcessorTypes { get; }
        void LoadCommandProcessors();
        void LoadCommandProcessors(Assembly[] assemblies);

        #endregion

        #region Message

        MessageResponse ClientMessageRequest(MessageRequest messageRequest);

        MessageResponse SendMessageToClient(string clientId, MessageRequest message);

        #endregion
    }

    /// <summary>
    /// 通信服务实现
    /// </summary>
    public class ICServer : _ICServer
    {
        #region Ctor

        public ICServer()
        {
            this.Clients = new ClientConcurrentDictionary();
            this.CommandProcessorTypes = new ConcurrentDictionary<string, Type>();
            // 默认加载程序中所有 processor
            this.LoadCommandProcessors();
        }

        #endregion

        #region Client

        public ClientConcurrentDictionary Clients { get; set; }

        public void ClientConnect(IClient client)
        {
            this.OnClientConnect?.Invoke(this, new ClientConnectedEventArgs(client));

            if (this.Clients.ContainsKey(client.ClientId)) throw new Exception("Client has exists. Client Id : " + client.ClientId);

            this.Clients.AddOrUpdate(client.ClientId, client, (key, val) =>
            {
                if (val != null)
                {
                    throw new Exception("Client has exists. Client Id : " + client.ClientId);
                }
                return client;
            });

            this.OnClientConnected?.Invoke(this, new ClientConnectedEventArgs(client));
        }

        public void ClientDisonnected(string clientId)
        {
            this.OnClientDisconnect?.Invoke(this, new ClientDisonnectedEventArgs(new Client(clientId, null)));

            while (true)
            {
                if (!this.Clients.ContainsKey(clientId)) break;

                IClient existedClient = null;
                if (this.Clients.TryRemove(clientId, out existedClient))
                {
                    this.OnClientDisconnected?.Invoke(this, new ClientDisonnectedEventArgs(existedClient));
                    existedClient.Close();
                    break;
                }

                System.Threading.Thread.Sleep(50);
            }
        }

        #endregion

        #region Supported event

        public event EventHandler<ClientConnectedEventArgs> OnClientConnected;
        public event EventHandler<MessageRequestReceivedEventArgs> OnMessageRequestReceived;
        public event EventHandler<MessageResponseReceivedEventArgs> OnMessageResponseReceived;
        public event EventHandler<ClientDisonnectedEventArgs> OnClientDisconnected;
        public event EventHandler<ClientConnectedEventArgs> OnClientConnect;
        public event EventHandler<ClientDisonnectedEventArgs> OnClientDisconnect;

        #endregion

        #region Command processor

        public ConcurrentDictionary<string, Type> CommandProcessorTypes { get; private set; }

        public void LoadCommandProcessors() => LoadCommandProcessors(AppDomain.CurrentDomain.GetAssemblies());

        public void LoadCommandProcessors(Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0) throw new ArgumentNullException("assemblies");

            ConcurrentDictionary<string, Type> _commandProcessors = new ConcurrentDictionary<string, Type>();
            foreach (Type t in assemblies.SelectMany(a => a.GetTypes()).Where(o => typeof(ICommandProcessor).IsAssignableFrom(o) && !o.IsAbstract && o.IsClass))
            {
                var commandProcessorDescription = t.GetCustomAttributes(typeof(CommandProcessorDescription), true).FirstOrDefault();

                if (commandProcessorDescription == null)
                {
                    throw new Exception("Must set CommandDescription attribute of your Command");
                }

                string commandId = (commandProcessorDescription as CommandProcessorDescription).CommandID;

                if (_commandProcessors.ContainsKey(commandId)) throw new Exception("Repeated command . " + commandId);

                _commandProcessors.AddOrUpdate(
                    commandId
                    , t
                    , (key, val) =>
                    {
                        val = null;
                        return t;
                    }
                    );
            }

            this.CommandProcessorTypes = _commandProcessors;
        }

        #endregion

        #region Message

        public MessageResponse ClientMessageRequest(MessageRequest messageRequest)
        {
            this.OnMessageRequestReceived?.Invoke(this, new MessageRequestReceivedEventArgs(messageRequest));

            try
            {
                if (!this.CommandProcessorTypes.ContainsKey(messageRequest.CommandId))
                {
                    throw new Exception("Unsupport command. " + messageRequest.CommandId);
                }

                var commandProcessor =
                    this.CommandProcessorTypes[messageRequest.CommandId]
                    .GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null)
                    .Invoke(null) as ICommandProcessor;

                var command = commandProcessor.ParseCommand(messageRequest.CommandRequestJson);

                return new MessageResponse()
                {
                    CommandResponseJson =
                        commandProcessor
                        .InternalProcess(commandProcessor.ParseCommand(messageRequest.CommandRequestJson), messageRequest)
                        .ToJson(),
                    MessageGuid = messageRequest.MessageGuid,
                    Success = true,
                    ResponseDate = DateTime.UtcNow
                };
            }
            catch (Exception e)
            {
                return new MessageResponse()
                {
                    MessageGuid = messageRequest.MessageGuid,
                    Success = false,
                    ErrorCode = "Internal Exception",
                    ErrorMessage = e.Message,
                    ResponseDate = DateTime.UtcNow
                };
            }
        }

        public MessageResponse SendMessageToClient(string clientId, MessageRequest message)
        {
            var response = Clients.GetClient(clientId).SendMessageToClient(message);
            this.OnMessageResponseReceived?.Invoke(this, new MessageResponseReceivedEventArgs(response));
            return response;
        }

        #endregion
    }

    #region Server Event Args

    public class ClientConnectEventArgs : EventArgs
    {
        public IClient Client { get; private set; }

        public ClientConnectEventArgs(IClient client)
        {
            this.Client = client;
        }
    }

    public class ClientConnectedEventArgs : ClientConnectEventArgs
    {
        public ClientConnectedEventArgs(IClient client)
            : base(client)
        {
        }
    }

    public class ClientDisonnectEventArgs : EventArgs
    {
        public IClient Client { get; private set; }

        public ClientDisonnectEventArgs(IClient client)
        {
            this.Client = client;
        }
    }

    public class ClientDisonnectedEventArgs : ClientDisonnectEventArgs
    {
        public ClientDisonnectedEventArgs(IClient client)
            : base(client)
        {
        }
    }

    public class MessageRequestReceivedEventArgs : EventArgs
    {
        public MessageRequest MessageRequest { get; private set; }

        public MessageRequestReceivedEventArgs(MessageRequest messageRequest)
        {
            this.MessageRequest = messageRequest;
        }
    }

    public class MessageResponseReceivedEventArgs : EventArgs
    {
        public MessageResponse MessageResponse { get; private set; }

        public MessageResponseReceivedEventArgs(MessageResponse messageResponse)
        {
            this.MessageResponse = messageResponse;
        }
    }

    #endregion
}
