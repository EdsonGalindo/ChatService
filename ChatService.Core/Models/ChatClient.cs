using ChatService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Core.Models
{
    public class ChatClient
    {
        public string Nickname { get; set; }
        public TcpClient SocketConnection { get; set; }
    }
}
