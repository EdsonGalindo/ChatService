using ChatService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatService.Core.Models
{
    public class ChatServer
    {
        public IPAddress IP { get; set; }
        public int Port { get; set; }
        public TcpListener Socket { get; set; }
        public List<ChatClient> ChatClients { get; set; } = new List<ChatClient>();
        public Thread ChatServerThread { get; set; }
    }
}
