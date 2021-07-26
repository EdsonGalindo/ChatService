using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("##################");
            Console.WriteLine("#Chat Server v1.0#");
            Console.WriteLine("##################");

            var chatServer = new ChatService.Core.Services.ChatServerService();
            chatServer.IP = IPAddress.Parse("127.0.0.1");
            chatServer.Port = 8182;
            chatServer.Start();

            Console.WriteLine(@"Press ""Enter"" to Stop the Chat Server");
            Console.ReadLine();
            Console.WriteLine("Stopping Chat Server... Please Wait process finish...");
            chatServer.Stop();
        }
    }
}
