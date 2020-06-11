using IC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace IC.Tcp
{
    public struct ICTcpHeader
    {
        string Header { get; set; }
        int DataLength { get; set; }
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

            using (var tcpClient = listener.EndAcceptTcpClient(ar))
            {
                string clientId = System.Guid.NewGuid().ToString();

                base.ClientConnect(new Client(clientId, new ICTcpConnection(tcpClient) { }));

                using (var networkStream = tcpClient.GetStream())
                {
                    while (true)
                    {
                        try
                        {
                            byte[] bytes = new byte[tcpClient.ReceiveBufferSize];

                            // read 同步阻塞进行，一定要保证客户端同步写入。如果真的错误怎么处理？？
                            // 原因应该发送  HEADER+LENG+DATA, 但是接收一半就错乱了如何处理？？？
                            // 每次碰到头就重新开始？那得每次判断数据包里是否有头？2个包组合起来才有一个头？
                            int readSize = networkStream.Read(bytes, 0, tcpClient.ReceiveBufferSize);

                            if (readSize == 0) break;

                            if (readSize > 0)
                            {
                                var messageResponse = base.ClientMessageRequest(new MessageRequest()
                                {
                                    CommandId = "C001",
                                    MessageGuid = System.Guid.NewGuid(),
                                    CommandRequestJson = "{\"Message\":\"" + System.Text.Encoding.UTF8.GetString(bytes, 0, readSize) + "\"}"
                                });

                                var resBytes = messageResponse.SerializeToBinaryFormatter();
                                networkStream.Write(resBytes, 0, resBytes.Length);
                            }
                        }
                        catch (System.IO.IOException ie)
                        {

                            break;
                            // client closed
                        }
                        catch (System.ObjectDisposedException ode)
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

            listener.BeginAcceptTcpClient(TcpAcceptAsyncCallback, listener);
        }
    }
}
