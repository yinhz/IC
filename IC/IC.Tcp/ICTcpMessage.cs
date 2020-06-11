using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IC.Core;

namespace IC.Tcp
{
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ICTcpMessageHead
    {
        public string Header;
        public int DataLength;
    }

    public class ICTcpMessageContent
    {
        public ICTcpMessageContent(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            this.Data = obj.SerializeToBinaryFormatter();
        }
        public int DataLength => Data.Length;
        public byte[] Data { get; set; }
    }
    public class ICTcpMessageContent<T> : ICTcpMessageContent
        where T : class
    {
        public ICTcpMessageContent(T obj)
            : base(obj)
        {

        }
    }
}
