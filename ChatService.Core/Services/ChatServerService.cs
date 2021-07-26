using ChatService.Core.Commons;
using ChatService.Core.Interfaces;
using ChatService.Core.Models;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatService.Core.Services
{
    public class ChatServerService : ChatServer, IChatServerService
    {
        public bool IsChatServerActive { get; private set; } = false;
        private Int32 maxArrayBytes { get; } = 10025;
        private string messageForAllText { get; } = "ALL";
        private string destinataryCommand { get; } = "/p ";

        public void AddChatServerWorker()
        {
            this.ChatServerThread = new Thread(ChatServerWorker);
            this.ChatServerThread.Start();

            if(this.ChatServerThread.IsAlive)
                this.IsChatServerActive = true;
        }

        public void RemoveChatServerWorker()
        {
            try
            {
                this.ChatServerThread.Abort();
            }
            catch { }

            this.IsChatServerActive = false;
        }

        public void AddClient(ChatClient chatClient)
        {
            this.ChatClients.Add(chatClient);
        }

        public void AddClientMessagesWorker(ChatClient chatClient)
        {
            var chatClientThread = new Thread(() => ClientMessagesWorker(chatClient));
            chatClientThread.Start();
        }

        public void ClientMessagesWorker(ChatClient chatClient)
        {
            var chatMessage = new ChatMessage();
            var newMessageBytes = new byte[maxArrayBytes];
            var isClientConnectionActive = true;

            while (isClientConnectionActive)
            {
                Thread.Sleep(500);

                isClientConnectionActive = chatClient.SocketConnection.Connected;

                if (!isClientConnectionActive || chatClient.SocketConnection.Available == 0)
                    continue;

                var chatClientStream = chatClient.SocketConnection.GetStream();
                var receivedBufferSize = chatClient.SocketConnection.ReceiveBufferSize;

                chatClientStream.Read(newMessageBytes, 0, maxArrayBytes);
                
                var newMessage = Encoding.ASCII.GetString(newMessageBytes);
                newMessage = newMessage.Replace("\0", string.Empty);

                chatMessage.From = chatClient.Nickname;
                chatMessage.To = IndentifiesMessageDestinatary(newMessage);
                chatMessage.Message = RemoveMessageCommand(newMessage);
                chatMessage.Type = (chatMessage.To == this.messageForAllText ? ChatMessagesTypes.MessageType.MessageToAll : 
                    ChatMessagesTypes.MessageType.MessageFromTo);

                chatMessage = ExecuteSessionCommand(chatMessage, chatClient);

                this.ChatServerMessenger(chatMessage);
            }
        }

        private ChatMessage ExecuteSessionCommand(ChatMessage chatMessage, ChatClient chatClient)
        {
            if (chatMessage.Message.Contains("/exit"))
            {
                var exitCommandMessage = new ChatMessage();
                exitCommandMessage.From = chatMessage.From;
                exitCommandMessage.Type = ChatMessagesTypes.MessageType.ExitChat;
                
                chatClient.SocketConnection.Close();
                this.RemoveClient(chatClient);
                return exitCommandMessage;
            }

            return chatMessage;
        }

        private string RemoveMessageCommand(string newMessage)
        {
            var messageDestinatary = IndentifiesMessageDestinatary(newMessage);
            var adjustedMesssage = string.Empty;

            if (messageDestinatary == this.messageForAllText)
                return newMessage;

            var replaceTargetText = $"{this.destinataryCommand}{messageDestinatary}";
            adjustedMesssage = newMessage.Replace(replaceTargetText, string.Empty).Trim();

            return adjustedMesssage;
        }

        private string IndentifiesMessageDestinatary(string newMessage)
        {
            var messageDestinarary = this.messageForAllText;
            var destinataryCommand = this.destinataryCommand;

            if (!newMessage.Contains(destinataryCommand))
                return messageDestinarary;

            var messageWithoutCommand = newMessage.Substring(newMessage.IndexOf(destinataryCommand));
            if (messageWithoutCommand.Length == 0)
                return messageDestinarary;

            var messageSplit = messageWithoutCommand.Split(' ');
            if (messageSplit.Length <= 1)
                return messageDestinarary;

            var possibleDestinatary = messageSplit[1];
            var isChatUserNickname = false;
            foreach (var chatClient in this.ChatClients)
            {
                if (possibleDestinatary.ToUpper() != chatClient.Nickname.ToUpper())
                    continue;

                isChatUserNickname = true;
                break;
            }

            if (isChatUserNickname)
                messageDestinarary = possibleDestinatary;

            return messageDestinarary;
        }

        public void ChatServerMessenger(ChatMessage chatMessage)
        {
            var formattedMessage = FormatChatMessage(chatMessage);

            foreach (var chatClient in this.ChatClients)
            {
                NetworkStream chatClientStream = chatClient.SocketConnection.GetStream();
                var newMessageBytes = Encoding.ASCII.GetBytes(formattedMessage);

                chatClientStream.Write(newMessageBytes, 0, newMessageBytes.Length);
                chatClientStream.Flush();
            }

            Console.WriteLine(FormatChatMessage(chatMessage));
        }

        private string FormatChatMessage(ChatMessage chatMessage)
        {
            var formattedMessage = string.Empty;

            switch (chatMessage.Type)
            {
                case ChatMessagesTypes.MessageType.AdviceNewChatUser:
                    {
                        formattedMessage = $"({DateTime.Now.ToString("HH:mm:ss")}) Chat_Server says: {chatMessage.From} entered in room!";
                        break;
                    }
                case ChatMessagesTypes.MessageType.MessageToAll:
                    {
                        formattedMessage = $"({DateTime.Now.ToString("HH:mm:ss")}) {chatMessage.From} says for {messageForAllText}: {chatMessage.Message}";
                        break;
                    }
                case ChatMessagesTypes.MessageType.MessageFromTo:
                    {
                        formattedMessage = $"({DateTime.Now.ToString("HH:mm:ss")}) {chatMessage.From} says to {chatMessage.To}: {chatMessage.Message}";
                        break;
                    }
                case ChatMessagesTypes.MessageType.ExitChat:
                    {
                        formattedMessage = $"({DateTime.Now.ToString("HH:mm:ss")}) Chat_Server says: {chatMessage.From} exited chat room!";
                        break;
                    }
                default: break;
            }

            return formattedMessage;
        }

        public void ChatServerWorker()
        {            
            var chatMessage = new ChatMessage();

            while (this.IsChatServerActive)
            {
                Thread.Sleep(500);
                this.IsChatServerActive = this.Socket.Server.IsBound;

                if (!this.IsChatServerActive)
                    break;

                var chatClient = new ChatClient();

                try
                {
                    chatClient.SocketConnection = this.Socket.AcceptTcpClient();
                }
                catch { break; }

                NetworkStream networkStream = chatClient.SocketConnection.GetStream();

                var receiveBufferSize = (int)chatClient.SocketConnection.ReceiveBufferSize;

                if (receiveBufferSize == 0)
                    continue;

                var receiveBufferBytes = new byte[maxArrayBytes];

                networkStream.Read(receiveBufferBytes, 0, maxArrayBytes);

                var clientReceivedData = Encoding.ASCII.GetString(receiveBufferBytes);
                clientReceivedData = clientReceivedData.Replace("\0", string.Empty);
                chatClient.Nickname = clientReceivedData.Trim();

                this.AddClient(chatClient);
                                
                chatMessage.From = clientReceivedData;
                chatMessage.Type = ChatMessagesTypes.MessageType.AdviceNewChatUser;

                ChatServerMessenger(chatMessage);

                AddClientMessagesWorker(chatClient);
            }
        }

        public void RemoveClient(ChatClient chatClient)
        {
            this.ChatClients.Remove(chatClient);
        }

        public void RemoveClientMessagesWorker()
        {
            throw new NotImplementedException();
        }

        public async Task Start()
        {
            if (this.IP == null || this.Port == 0)
                return;

            var newSocket = SocketOperations.StartNewTcpListener(this.IP, this.Port);

            if (!newSocket.Server.IsBound)
                return;

            this.Socket = newSocket;

            AddChatServerWorker();

            Console.WriteLine("Chat Server Started!");
        }

        public void Stop()
        {
            this.StopWaiting(this.Socket, 5000);
            this.Socket.Server.Dispose();
            RemoveChatServerWorker();

            Console.WriteLine("Chat Server Stoped!");
        }

        /// <summary>
        /// Stops the Chat Server awaiting specified time in milliseconds, avoiding return before all processes are finished
        /// </summary>
        /// <param name="tcpListener">A TcpListener object</param>
        /// <param name="milisecondsWait">The time in milliseconds to wait</param>
        public void StopWaiting(TcpListener tcpListener, int milisecondsWait)
        {
            var timeWatch = Stopwatch.StartNew();
            var chatServerRunning = this.IsChatServerActive;

            tcpListener.Stop();

            var timePassed = timeWatch.ElapsedMilliseconds;

            while (timePassed <= milisecondsWait && chatServerRunning)
            {
                chatServerRunning = this.IsChatServerActive;
                timePassed = timeWatch.ElapsedMilliseconds;
            }

            timeWatch.Stop();
        }
    }
}
