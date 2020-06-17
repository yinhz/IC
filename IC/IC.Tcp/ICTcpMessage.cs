using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IC.Core;

namespace IC.Tcp
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct IC_TCP_MESSAGE_STRUCT
    {
        public const int HeaderLength = 34;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 28)]
        public string StartToken;
        [MarshalAs(UnmanagedType.I1, SizeConst = 1)]
        public MessageType MessageType;
        [MarshalAs(UnmanagedType.I1, SizeConst = 1)]
        public MessageFormat MessageFormat;
        [MarshalAs(UnmanagedType.I4, SizeConst = 4)]
        public int DataLength;

        public static IC_TCP_MESSAGE_STRUCT CreateICTcpMessageStruct(MessageType messageType, int dataLength, MessageFormat messageFormat = MessageFormat.Binary)
        {
            return new IC_TCP_MESSAGE_STRUCT()
            {
                StartToken = IC_TCP_MESSAGE_STRUCT.StartTokenStr,
                MessageType = messageType,
                DataLength = dataLength,
                MessageFormat = messageFormat
            };
        }
        public const string StartTokenStr = "IC.Tcp.IC_TCP_MESSAGE_HEADER";
        public const string EndTokenStr = "IC.Tcp.IC_TCP_MESSAGE_END";
        public static readonly byte[] IC_TCP_MESSAGE_END_TOKEN = EndTokenStr.ToBytes();
        public static readonly int IC_TCP_MESSAGE_END_TOKEN_LENGTH = IC_TCP_MESSAGE_END_TOKEN.Length;
        
        #region Tcp message

        public static byte[] CreateTcpRequestMessage(MessageRequest messageRequest, MessageFormat messageFormat = MessageFormat.Binary)
        {
            return CreateTcpMessage<MessageRequest>(messageRequest, MessageType.Request, messageFormat);
        }
        public static byte[] CreateTcpResponseMessage(MessageResponse messageResponse, MessageFormat messageFormat = MessageFormat.Binary)
        {
            return CreateTcpMessage<MessageResponse>(messageResponse, MessageType.Response, messageFormat);
        }
        public static byte[] CreateTcpMessage<T>(T message, MessageType messageType, MessageFormat messageFormat = MessageFormat.Binary)
        {
            if (message == null)
                throw new ArgumentNullException("messageRequest");

            byte[] data = null;
            byte[] messageBytes = null;
            if (messageFormat == MessageFormat.Binary)
            {
                messageBytes = message.SerializeToBinaryFormatter();
                data = new byte[IC_TCP_MESSAGE_STRUCT.HeaderLength + messageBytes.Length + IC_TCP_MESSAGE_STRUCT.IC_TCP_MESSAGE_END_TOKEN_LENGTH];
            }
            else if (messageFormat == MessageFormat.Json)
            {
                messageBytes = Utils.JsonToBytes(message.ToJson());
                data = new byte[IC_TCP_MESSAGE_STRUCT.HeaderLength + messageBytes.Length];
            }
            else
                throw new Exception("Unsupport message format!");

            IC_TCP_MESSAGE_STRUCT header = IC_TCP_MESSAGE_STRUCT.CreateICTcpMessageStruct(MessageType.Request, messageBytes.Length, messageFormat);

            Buffer.BlockCopy(Utils.StructToBytes(header), 0, data, 0, IC_TCP_MESSAGE_STRUCT.HeaderLength);
            Buffer.BlockCopy(messageBytes, 0, data, IC_TCP_MESSAGE_STRUCT.HeaderLength, messageBytes.Length);
            Buffer.BlockCopy(IC_TCP_MESSAGE_STRUCT.IC_TCP_MESSAGE_END_TOKEN, 0, data, IC_TCP_MESSAGE_STRUCT.HeaderLength + messageBytes.Length, IC_TCP_MESSAGE_STRUCT.IC_TCP_MESSAGE_END_TOKEN_LENGTH);

            return data;
        }

        #endregion
    }
}
