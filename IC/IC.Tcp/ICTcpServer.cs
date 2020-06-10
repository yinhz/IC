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
        private string tcp_server_ipaddress = "127.0.0.1";
        private int tcp_listener_port = 36001;

        public ICTcpServer()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Parse(tcp_server_ipaddress), tcp_listener_port);
            tcpListener.Start();

            tcpListener.BeginAcceptTcpClient(new AsyncCallback((ar) =>
            {
                var listener = (ar.AsyncState as TcpListener);
                var tcpClient = listener.EndAcceptTcpClient(ar);
                
            }), tcpListener);
        }
    }
}
