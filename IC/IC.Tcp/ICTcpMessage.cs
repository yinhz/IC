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
        public const string TokenStr = "IC.Tcp.IC_TCP_MESSAGE_HEADER";
        public const int HeaderLength = 29;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string Token;
        [MarshalAs(UnmanagedType.I1, SizeConst = 1)]
        public MessageType MessageType;
        [MarshalAs(UnmanagedType.I4, SizeConst = 4)]
        public int DataLength;

        public static IC_TCP_MESSAGE_HEADER CreateICTcpMessageHeader(MessageType messageType, int dataLength)
        {
            return new IC_TCP_MESSAGE_HEADER()
            {
                Token = IC_TCP_MESSAGE_HEADER.TokenStr,
                MessageType = messageType,
                DataLength = dataLength
            };
        }
    }
}
