using System;
using System.Threading.Tasks;

namespace IC.Core
{
    public interface IConnection : IDisposable
    {
        IConnectionContext ConnectionContext { get; }
        string SessionId { get; }
        Task Send(MessageRequest messageRequest);
        void Close();
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageRequest MessageRequest { get; private set; }
        public MessageReceivedEventArgs(MessageRequest messageRequest)
        {
            this.MessageRequest = messageRequest;
        }
    }

    public class MessageSendedEventArgs : EventArgs
    {
        public MessageRequest MessageRequest { get; private set; }
        public MessageSendedEventArgs(MessageRequest messageRequest)
        {
            this.MessageRequest = messageRequest;
        }
    }

    public interface IConnectionContext
    {
        string MasterEquipmentCode { get; set; }
        string EquipmentCode { get; set; }
    }
}