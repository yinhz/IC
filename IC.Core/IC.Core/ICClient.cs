using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace IC.Core
{
    public interface IClient : IDisposable
    {
        string ClientId { get; }
        IConnection Connection { get; }
        Task Send(MessageRequest message);

        void Close();
    }

    public class Client : IClient
    {
        public Client(IConnection connection, string clientId)
        {
            this.Connection = connection;
            this.ClientId = clientId;
        }
        public IConnection Connection { get; private set; }
        public string ClientId { get; private set; }

        public void Close()
        {
            this.Connection?.Close();
        }

        public void Dispose()
        {
            this.Close();
        }

        public Task Send(MessageRequest message)
        {
            return this.Connection.Send(message);
        }
    }

    public class ClientDictionary : ConcurrentDictionary<string, IClient>
    {
        public IClient GetClient(string sessionId)
        {
            var client = this[sessionId];

            if (client == null)
                throw new Exception("Can not get Client");

            return client;
        }
    }
}
