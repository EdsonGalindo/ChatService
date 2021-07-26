using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Core.Commons
{
    public class SocketOperations
    {
        public static TcpListener StartNewTcpListener(IPAddress listeningIP, int listeningPort)
        {
            var tcpListener = new TcpListener(listeningIP, listeningPort);
            tcpListener.Start();
            return tcpListener;
        }

        public static TcpClient StartNewTcpClient(string serverHostName, int serverHostPort)
        {
            try
            {
                return new TcpClient(serverHostName, serverHostPort);
            }
            catch { return new TcpClient(); }
        }
    }
}
