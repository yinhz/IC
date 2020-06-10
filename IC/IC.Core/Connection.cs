using System;
using System.Threading.Tasks;

namespace IC.Core
{
    public interface IConnection : IDisposable
    {
        string SessionId { get; }
        MessageResponse SendMessageToClient(MessageRequest messageRequest);
        void Close();
    }
}