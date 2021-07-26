using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatService.Core.Services;

namespace ChatService.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("##################");
            Console.WriteLine("#Chat Client v1.0#");
            Console.WriteLine("##################");

            Console.WriteLine("Hello there! To get in please choose a Nickname:");
            var nickname = Console.ReadLine();

            // Create a new Chat Client instance
            var chatServerHostName = "127.0.0.1";
            var chatServerHostPort = 8182;
            var chatClientService = new ChatClientService(nickname, chatServerHostName, chatServerHostPort);
            chatClientService.Connect();
        }
    }
}
