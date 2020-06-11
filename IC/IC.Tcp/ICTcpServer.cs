using IC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace IC.Tcp
{
    public struct ICTcpHeaderStruct
    {
        public string Header { get; set; }
        public int DataLength { get; set; }
    }

    public class ICTcpServer : ICServer
    {
        public System.Collections.Concurrent.ConcurrentBag<TcpClient> tcpClients = new System.Collections.Concurrent.ConcurrentBag<TcpClient>();

        public delegate void DataReceivedDelegate(byte[] bytes, string message);
        public DataReceivedDelegate OnDataReceived;

        public ICTcpServer()
        {

        }

        public void Start(string server, int port)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Parse(server), port);
            tcpListener.Start();

            tcpListener.BeginAcceptTcpClient(TcpAcceptAsyncCallback, tcpListener);
        }

        public void TcpAcceptAsyncCallback(IAsyncResult ar)
        {
            var listener = (ar.AsyncState as TcpListener);

            try
            {
                var tcpClient = listener.EndAcceptTcpClient(ar);

                Task.Run(() =>
                {
                    try
                    {
                        string clientId = System.Guid.NewGuid().ToString();

                        base.ClientConnect(new Client(clientId, new ICTcpConnection(tcpClient) { }));

                        using (var networkStream = tcpClient.GetStream())
                        {
                            var clientBufferQueue = new System.Collections.Generic.Queue<byte>();

                            IC_TCP_MESSAGE_HEADER messageHeader = default(IC_TCP_MESSAGE_HEADER);

                            while (true)
                            {
                                try
                                {
                                    #region 把数据读到队列里
                                    byte[] bytes = new byte[tcpClient.ReceiveBufferSize];

                                    int readSize = networkStream.Read(bytes, 0, tcpClient.ReceiveBufferSize);

                                    if (readSize == 0) break;

                                    if (readSize > 0)
                                    {
                                        for (int i = 0; i < readSize; i++)
                                        {
                                            clientBufferQueue.Enqueue(bytes[i]);
                                        }
                                    }
                                    #endregion

                                    #region 处理队列中数据

                                    // 当队列长度比定义的TCP头大的时候
                                    if (clientBufferQueue.Count > IC_TCP_MESSAGE_HEADER.HeaderLength)
                                    {
                                        byte[] headerBytes = new byte[IC_TCP_MESSAGE_HEADER.HeaderLength];
                                        // 取出消息头
                                        for (int i = 0; i < headerBytes.Length; i++)
                                        {
                                            headerBytes[i] = clientBufferQueue.Dequeue();
                                        }
                                        messageHeader = Utils.BytesToStruct<IC_TCP_MESSAGE_HEADER>(headerBytes);
                                    }

                                    #endregion

                                    #region 如果有数据长度，且 当前的 TCP 消息头已组成，则可以读取内容
                                    if (messageHeader.DataLength > 0 && clientBufferQueue.Count >= messageHeader.DataLength)
                                    {
                                        byte[] messageBytes = new byte[messageHeader.DataLength];
                                        for (int i = 0; i < messageHeader.DataLength; i++)
                                        {
                                            messageBytes[i] = clientBufferQueue.Dequeue();
                                        }

                                        if (messageHeader.MessageType == MessageType.Request)
                                        {
                                            MessageRequest messageRequest = Utils.DeserializeToObject<MessageRequest>(messageBytes);

                                            var messageResponse = base.ClientMessageRequest(messageRequest);
                                        }
                                        else if (messageHeader.MessageType == MessageType.Response)
                                        {
                                            MessageResponse messageRequest = Utils.DeserializeToObject<MessageResponse>(messageBytes);
                                        }
                                    }
                                    #endregion
                                }
                                catch (System.IO.IOException ie)
                                {
                                    break;
                                    // client closed
                                }
                                catch (Exception e)
                                {
                                    break;
                                }
                            }
                        }

                        base.ClientDisonnected(clientId);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception. " + e.Message);
                        tcpClient.Close();
                    }
                });

            }
            //An existing connection was forcibly closed by the remote host
            catch (System.Net.Sockets.SocketException e)
            {
                Console.WriteLine("Exception. " + e.Message);
            }

            listener.BeginAcceptTcpClient(TcpAcceptAsyncCallback, listener);
        }
    }
}