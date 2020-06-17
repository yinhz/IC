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
            var requestMessageBytes = IC_TCP_MESSAGE_STRUCT.CreateTcpRequestMessage(messageRequest);
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
    }
}