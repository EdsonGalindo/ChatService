using ChatService.Core.Interfaces;
using ChatService.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatService.Core.Services
{
    public class ChatClientService : ChatClient, IChatClientService
    {
        private string ServerHostName { get; set; }
        private int ServerPort { get; set; }
        public bool IsChatClientConnected { get; private set; } = false;
        private Int32 maxArrayBytes { get; } = 10025;

        /// <summary>
        /// Inite a new Char Client
        /// </summary>
        /// <param name="nickname">The nickname that will be used in the chat</param>
        /// <param name="serverHostName">The host name of the chat server</param>
        /// <param name="serverPort">The host port number</param>
        public ChatClientService(string nickname, string serverHostName, int serverPort)
        {
            this.Nickname = nickname;
            this.ServerHostName = serverHostName;
            this.ServerPort = serverPort;
        }

        public async Task Connect()
        {
            this.SocketConnection = Commons.SocketOperations.StartNewTcpClient(ServerHostName, ServerPort);
            IsChatClientConnected = this.SocketConnection.Connected;

            if (!IsChatClientConnected)
            {
                Console.WriteLine("Connection to server failed :( Please try again later.");
                return;
            }

            AddMessageReceiverWorker();
            AddMessageSenderWorker();

            Console.WriteLine("Connected to the Chat Server successfully!");

            var newMessage = this.Nickname;
            var sendMessageStream = this.SocketConnection.GetStream();
            var newMessageStream = Encoding.ASCII.GetBytes(newMessage);
            sendMessageStream.Write(newMessageStream, 0, newMessageStream.Length);
            sendMessageStream.Flush();
        }

        public void Disconnect()
        {
            this.DisconnectWaiting(this.SocketConnection, 5000);
            this.SocketConnection.Dispose();
            IsChatClientConnected = false;

            Console.Clear();
            Console.WriteLine("Disconnected from the Chat Server successfully! See you soon!");
        }

        public void AddMessageReceiverWorker()
        {
            if (!IsChatClientConnected)
                return;

            var messageReceiverWorker = new Thread(MessageReceiverWorker);
            messageReceiverWorker.Start();
        }

        public void MessageReceiverWorker()
        {
            NetworkStream receivedMessage;
            var receivedBufferSize = 0;
            var receivingMessageStream = new byte[maxArrayBytes];
            var newMessage = string.Empty;

            while (IsChatClientConnected) 
            {
                Thread.Sleep(500);
                IsChatClientConnected = this.SocketConnection.Connected;

                if (!IsChatClientConnected)
                    break;

                if (this.SocketConnection.Available == 0)
                    continue;

                receivedMessage = this.SocketConnection.GetStream();
                receivedBufferSize = this.SocketConnection.ReceiveBufferSize;

                receivedMessage.Read(receivingMessageStream, 0, maxArrayBytes);
                newMessage = Encoding.ASCII.GetString(receivingMessageStream);
                newMessage = newMessage.Replace("\0", string.Empty);
                receivingMessageStream = new byte[maxArrayBytes];

                Console.WriteLine(newMessage);
            }
        }

        public void AddMessageSenderWorker()
        {
            if (!IsChatClientConnected)
                return;

            var messageSenderWorker = new Thread(MessageSender);
            messageSenderWorker.Start();
        }

        public void MessageSender()
        {
            NetworkStream sendMessageStream;
            var newMessage = string.Empty;
            var newMessageStream = new byte[maxArrayBytes];

            while (IsChatClientConnected)
            {
                newMessage = Console.ReadLine();

                IsChatClientConnected = this.SocketConnection.Connected;

                if (!IsChatClientConnected)
                    break;

                if (string.IsNullOrWhiteSpace(newMessage))
                    continue;

                sendMessageStream = this.SocketConnection.GetStream();
                newMessageStream = Encoding.ASCII.GetBytes(newMessage);
                sendMessageStream.Write(newMessageStream, 0, newMessageStream.Length);
                sendMessageStream.Flush();

                ExecuteSessionCommand(newMessage);
                if (!IsChatClientConnected)
                    break;
            }
        }

        private void ExecuteSessionCommand(string clientMessage)
        {
            if (clientMessage.Contains("/exit"))
            {
                this.Disconnect();
                return;
            }
        }

        /// <summary>
        /// Stops the Chat Server awaiting specified time in milliseconds, avoiding return before all processes are finished
        /// </summary>
        /// <param name="tcpListener">A TcpListener object</param>
        /// <param name="milisecondsWait">The time in milliseconds to wait</param>
        public void DisconnectWaiting(TcpClient tcpClient, int milisecondsWait)
        {
            var timeWatch = Stopwatch.StartNew();
            var chatClientConnected = this.IsChatClientConnected;

            tcpClient.Client.Disconnect(false);

            var timePassed = timeWatch.ElapsedMilliseconds;

            while (timePassed <= milisecondsWait && chatClientConnected)
            {
                chatClientConnected = this.IsChatClientConnected;
                timePassed = timeWatch.ElapsedMilliseconds;
            }

            timeWatch.Stop();
        }
    }
}
