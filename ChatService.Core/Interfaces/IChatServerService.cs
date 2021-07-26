using ChatService.Core.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ChatService.Core.Interfaces
{
    public interface IChatServerService
    {
        Task Start();
        void Stop();
        void AddChatServerWorker();
        void RemoveChatServerWorker();
        void ChatServerWorker();
        void AddClient(ChatClient chatClient);
        void RemoveClient(ChatClient chatClient);
        void ChatServerMessenger(ChatMessage chatMessage);
        void AddClientMessagesWorker(ChatClient chatClient);
        void ClientMessagesWorker(ChatClient chatClient);
        void RemoveClientMessagesWorker();
    }
}
