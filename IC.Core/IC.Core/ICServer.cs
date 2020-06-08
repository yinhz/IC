using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace IC.Core
{
    public interface _ICServer
    {
        ClientDictionary Clients { get; set; }
        void RegisterClient(IClient client);
        void RemoveClient(string clientId);
        Task SendToClient(Client client, MessageRequest message);
        Task SendToClient(string clientId, MessageRequest message);
        ConcurrentDictionary<string, ICommandProcessor> CommandProcessors { get; }
        void BuildCommandProcessors();
        void BuildCommandProcessors(Assembly[] assemblies);
        void ReplaceCommandProcessors(ConcurrentDictionary<string, ICommandProcessor> commandProcessors);
    }

    public class ICServer : _ICServer
    {
        public ClientDictionary Clients { get; set; }

        public ConcurrentDictionary<string, ICommandProcessor> CommandProcessors { get; private set; }

        public ICServer()
        {
            this.Clients = new ClientDictionary();
            this.CommandProcessors = new ConcurrentDictionary<string, ICommandProcessor>();
            // 默认加载程序中所有 processor
            this.BuildCommandProcessors();
        }

        public void RegisterClient(IClient client)
        {
            this.Clients.AddOrUpdate(client.ClientId, client, (key, val) =>
            {
                val?.Close();
                return client;
            });
        }

        public Task SendToClient(Client client, MessageRequest message)
        {
            return client.Send(message);
        }

        public Task SendToClient(string clientId, MessageRequest message)
        {
            return Clients.GetClient(clientId).Send(message);
        }
        public void BuildCommandProcessors() => BuildCommandProcessors(AppDomain.CurrentDomain.GetAssemblies());
        public void BuildCommandProcessors(Assembly[] assemblies)
        {
            ConcurrentDictionary<string, ICommandProcessor> _commandProcessors = new ConcurrentDictionary<string, ICommandProcessor>();
            foreach (Type t in assemblies.SelectMany(a => a.GetTypes())
                                            .Where(
                                                o =>
                                                    typeof(ICommandProcessor).IsAssignableFrom(o)
                                                    && !o.IsAbstract
                                                    && o.IsClass
                                                   )
            )
            {
                var commandProcessorDescription = t.GetCustomAttributes(typeof(CommandProcessorDescription), true).FirstOrDefault();

                if (commandProcessorDescription == null)
                {
                    throw new Exception("Must set CommandDescription attribute of your Command");
                }

                _commandProcessors.AddOrUpdate(
                    (commandProcessorDescription as CommandProcessorDescription).CommandID
                    , t as ICommandProcessor
                    , (key, val) =>
                    {
                        val = null;
                        return t as ICommandProcessor;
                    }
                    );
            }

            this.CommandProcessors = _commandProcessors;
        }

        public void ReplaceCommandProcessors(ConcurrentDictionary<string, ICommandProcessor> commandProcessors)
        {
            this.CommandProcessors = commandProcessors;
        }

        public void RemoveClient(string clientId)
        {
            while (true)
            {
                if (!this.Clients.ContainsKey(clientId))
                {
                    break;
                }

                IClient existsClient = null;
                if (this.Clients.TryRemove(clientId, out existsClient))
                {
                    existsClient.Close();
                    break;
                }

                System.Threading.Thread.Sleep(50);
            }
        }
    }
}
