using IC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IC.Tcp.Client
{
    public class ICTcpClient : ICClient
    {
        private TcpClient tcpClient;
        private IPEndPoint remoteEP;

        public ICTcpClient(string clientId, IPEndPoint remoteEP, MessageReceivedDelegate messageReceived)
            : base(clientId, messageReceived)
        {
            this.remoteEP = remoteEP ?? throw new ArgumentException("remoteEP");
        }

        public override void Close()
        {
            if (tcpClient != null)
            {
                tcpClient.Dispose();
            }
        }

        public override void Dispose()
        {
            this.Close();
        }
        
        protected override void DoOpen()
        {
            if (this.tcpClient.Connected)
                throw new Exception("Socket has been connected");

            this.tcpClient.Connect(this.remoteEP);
        }

        protected override void DoRegister()
        {
            return;
        }

        protected override MessageResponse DoSendMessage(MessageRequest messageRequest)
        {
            var requestMessageBytes = this.CreateTcpRequestMessage<MessageRequest>(messageRequest);
            this.tcpClient.GetStream().Write(requestMessageBytes, 0, requestMessageBytes.Length);

            // 怎么得到返回值

            return null;
        }

        protected override Task<MessageResponse> DoSendMessageAsync(MessageRequest messageRequest)
        {
            return Task.Run(() =>
            {
                return this.DoSendMessage(messageRequest);
            });
        }

        protected override void Reset()
        {
            try
            {
                this.tcpClient?.Dispose();
            }
            finally
            {
                this.tcpClient = null;
                this.Registered = false;
            }

            this.tcpClient = new TcpClient();
        }

        #region MessageHandle

        private byte[] CreateTcpRequestMessage<T>(MessageRequest messageRequest, MessageFormat messageFormat = MessageFormat.Binary)
        {
            return CreateTcpMessage<MessageRequest>(messageRequest, MessageType.Request, messageFormat);
        }
        private byte[] CreateTcpResponseMessage<T>(MessageResponse messageResponse, MessageFormat messageFormat = MessageFormat.Binary)
        {
            return CreateTcpMessage<MessageResponse>(messageResponse, MessageType.Response, messageFormat);
        }
        private byte[] CreateTcpMessage<T>(T message, MessageType messageType, MessageFormat messageFormat = MessageFormat.Binary)
        {
            if (message == null)
                throw new ArgumentNullException("messageRequest");

            byte[] data = null;
            byte[] messageBytes = null;
            if (messageFormat == MessageFormat.Binary)
            {
                messageBytes = message.SerializeToBinaryFormatter();
                data = new byte[IC_TCP_MESSAGE_HEADER.HeaderLength + messageBytes.Length + IC_TCP_MESSAGE_HEADER.IC_TCP_MESSAGE_END_TOKEN_LENGTH];
            }
            else if (messageFormat == MessageFormat.Json)
            {
                messageBytes = Utils.JsonToBytes(message.ToJson());
                data = new byte[IC_TCP_MESSAGE_HEADER.HeaderLength + messageBytes.Length];
            }
            else
                throw new Exception("Unsupport message format!");

            IC_TCP_MESSAGE_HEADER header = IC_TCP_MESSAGE_HEADER.CreateICTcpMessageHeader(MessageType.Request, messageBytes.Length, messageFormat);

            Buffer.BlockCopy(Utils.StructToBytes(header), 0, data, 0, IC_TCP_MESSAGE_HEADER.HeaderLength);
            Buffer.BlockCopy(messageBytes, 0, data, IC_TCP_MESSAGE_HEADER.HeaderLength, messageBytes.Length);
            Buffer.BlockCopy(IC_TCP_MESSAGE_HEADER.IC_TCP_MESSAGE_END_TOKEN, 0, data, IC_TCP_MESSAGE_HEADER.HeaderLength + messageBytes.Length, IC_TCP_MESSAGE_HEADER.IC_TCP_MESSAGE_END_TOKEN_LENGTH);

            return data;
        }

        #endregion
    }
}