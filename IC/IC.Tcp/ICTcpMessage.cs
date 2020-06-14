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
    public struct IC_TCP_MESSAGE_HEADER
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

        public static IC_TCP_MESSAGE_HEADER CreateICTcpMessageHeader(MessageType messageType, int dataLength, MessageFormat messageFormat = MessageFormat.Binary)
        {
            return new IC_TCP_MESSAGE_HEADER()
            {
                StartToken = IC_TCP_MESSAGE_HEADER.StartTokenStr,
                MessageType = messageType,
                DataLength = dataLength,
                MessageFormat = messageFormat
            };
        }
        public const string StartTokenStr = "IC.Tcp.IC_TCP_MESSAGE_HEADER";
        public const string EndTokenStr = "IC.Tcp.IC_TCP_MESSAGE_END";
        public static readonly byte[] IC_TCP_MESSAGE_END_TOKEN = EndTokenStr.ToBytes();
        public static readonly int IC_TCP_MESSAGE_END_TOKEN_LENGTH = IC_TCP_MESSAGE_END_TOKEN.Length;
    }
}
