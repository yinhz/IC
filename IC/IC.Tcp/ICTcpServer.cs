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

                        base.ClientConnect(new ICServerClient(clientId, new ICTcpConnection(tcpClient) { }));

                        using (var networkStream = tcpClient.GetStream())
                        {
                            // to-do 到消息里读取，只登记一次客户端
                            List<byte> clientBufferList = new List<byte>();

                            IC_TCP_MESSAGE_STRUCT messageHeader = default(IC_TCP_MESSAGE_STRUCT);

                            while (true)
                            {
                                try
                                {
                                    #region 把数据读到集合里

                                    // buffer 设置多长
                                    byte[] bytes = new byte[tcpClient.ReceiveBufferSize];

                                    int readSize = networkStream.Read(bytes, 0, tcpClient.ReceiveBufferSize);

                                    if (readSize == 0) break;

                                    if (readSize > 0)
                                    {
                                        clientBufferList.AddRange(bytes.Take(readSize));
                                    }
                                    #endregion

                                    #region 处理队列中数据

                                    // 当队列长度比定义的TCP头大的时候
                                    if (clientBufferList.Count > IC_TCP_MESSAGE_STRUCT.HeaderLength)
                                    {
                                        byte[] headerBytes = new byte[IC_TCP_MESSAGE_STRUCT.HeaderLength];
                                        // 取出消息头
                                        for (int i = 0; i < headerBytes.Length; i++)
                                        {
                                            headerBytes[i] = clientBufferList[i];
                                        }
                                        clientBufferList.RemoveRange(0, headerBytes.Length);
                                        messageHeader = Utils.BytesToStruct<IC_TCP_MESSAGE_STRUCT>(headerBytes);
                                    }

                                    #endregion

                                    #region 如果有数据长度，且 当前的 TCP 消息头已组成，则可以读取内容
                                    // to-do , 这中间出现问题如何处理？？
                                    // 字节数够了，但是 消息匹配不上，比如结尾校验没通过？
                                    if (messageHeader.DataLength > 0 && clientBufferList.Count >= (messageHeader.DataLength + IC_TCP_MESSAGE_STRUCT.IC_TCP_MESSAGE_END_TOKEN_LENGTH))
                                    {
                                        #region 数据一致性校验，偏移数据长度后取结尾 IC_TCP_MESSAGE_END_TOKEN_LENGTH 长度，转换字符后应该是 结束标记
                                        string endToken = clientBufferList.Skip(messageHeader.DataLength).Take(IC_TCP_MESSAGE_STRUCT.IC_TCP_MESSAGE_END_TOKEN_LENGTH).BytesToString();
                                        if (endToken != IC_TCP_MESSAGE_STRUCT.EndTokenStr)
                                        {
                                            // 如果一段时间内没匹配上？不会存在一段时间问题，要么客户端断开，要么字节会一直发送，这边有读取。
                                            // 匹配不上，放弃本次处理？// 断开客户端？
                                            // 为啥会匹配不上？? 
                                            // 匹配不上移除消息？？
                                            goto RemoveCurrentMessage;
                                            throw new Exception("Dont match end token!");
                                        }
                                        #endregion

                                        #region 获取 实际消息字节
                                        byte[] messageBytes = new byte[messageHeader.DataLength];
                                        for (int i = 0; i < messageHeader.DataLength; i++)
                                        {
                                            messageBytes[i] = clientBufferList[i];
                                        }
                                        #endregion

                                        #region 开始处理收到的消息
                                        try
                                        {
                                            MessageResponse messageResponse;

                                            if (messageHeader.MessageType == MessageType.Request)
                                            {
                                                // to-do  ， 阻塞 客户端的消息可以。当 服务端处理不过来的时候
                                                Task.Run(() =>
                                                {
                                                    MessageRequest messageRequest = null;

                                                    if (messageHeader.MessageFormat == MessageFormat.Binary)
                                                        messageRequest = Utils.DeserializeToObject<MessageRequest>(messageBytes);
                                                    else if (messageHeader.MessageFormat == MessageFormat.Json)
                                                        messageRequest = MessageUtils.FromMessageRequestJson(messageBytes.ToJson());

                                                    messageResponse = base.ClientMessageRequest(messageRequest);

                                                    Console.WriteLine("*********" + Utils.ToJson(messageResponse));

                                                    byte[] responseBytes = IC_TCP_MESSAGE_STRUCT.CreateTcpResponseMessage(messageResponse);

                                                    networkStream.Write(responseBytes, 0, responseBytes.Length);
                                                });
                                            }
                                            else if (messageHeader.MessageType == MessageType.Response)
                                            {
                                                //MessageResponse messageResponse = Utils.DeserializeToObject<MessageResponse>(messageBytes);
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            // to-do 消息解析失败？
                                            goto RemoveCurrentMessage;
                                            // to-do 消息处理失败？ 消息处理不会失败，ClientMessageRequest 永远会有 try catch.
                                            throw;
                                        }
                                    #endregion

                                    // 获取完成移除 客户端消息缓冲区数据
                                    RemoveCurrentMessage:
                                        clientBufferList.RemoveRange(0, messageHeader.DataLength + IC_TCP_MESSAGE_STRUCT.IC_TCP_MESSAGE_END_TOKEN_LENGTH);
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