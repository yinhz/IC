using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IC.Core
{
    public interface _ICServerClient : IDisposable
    {
        string ClientId { get; }

        string SessionId { get; }
        IConnection Connection { get; }
        MessageResponse SendMessageToClient(MessageRequest messageRequest);
        void Close();
    }
    
    public class ICServerClient : _ICServerClient
    {
        public ICServerClient(string clientId, IConnection connection)
        {
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException("clientId");

            if (connection == null)
                throw new ArgumentNullException("connection");

            this.ClientId = clientId;
            this.Connection = connection;
        }
        
        public override int GetHashCode()
        {
            return ("Client" + this.ClientId.ToString()).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var other = obj as ICServerClient;

            return this.ClientId == other.ClientId;
        }
        
        public IConnection Connection { get; private set; }

        public string ClientId { get; private set; }

        public string SessionId => this.Connection?.SessionId;

        public void Close()
        {
            this.Connection?.Close();
        }

        public void Dispose()
        {
            this.Close();
        }

        public MessageResponse SendMessageToClient(MessageRequest message)
        {
            return this.Connection.SendMessageToClient(message);
        }

        public bool Equals(ICServerClient x, ICServerClient y)
        {
            if (x == null || y == null) return false;
            return x?.ClientId == y?.ClientId;
        }

        public int GetHashCode(ICServerClient obj)
        {
            return obj.GetHashCode();
        }
    }

    public class ICServerClientConcurrentDictionary : ConcurrentDictionary<string, _ICServerClient>
    {
        public _ICServerClient GetClient(string clientId)
        {
            return this[clientId];
        }
    }
}
