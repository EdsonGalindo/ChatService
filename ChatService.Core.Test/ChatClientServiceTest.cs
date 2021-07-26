using ChatService.Core.Services;
using NUnit.Framework;
using System.Net;

namespace ChatService.Core.Test
{
    public class ChatClientServiceTest
    {
        ChatClientService ChatClientService;
        ChatServerService ChatServerService;

        [SetUp]
        public void Setup()
        {
            var chatServerHostName = "127.0.0.1";
            var chatServerHostPort = 8182;

            // Client Setup
            var nickname = "TakeUser";
            this.ChatClientService = new ChatClientService(nickname, chatServerHostName, chatServerHostPort);

            // Server setup
            this.ChatServerService = new ChatServerService();
            this.ChatServerService.IP = IPAddress.Parse(chatServerHostName);
            this.ChatServerService.Port = chatServerHostPort;
            ChatServerService.Start();
        }

        [Test]
        public void ShouldConnectChatClientToChatServerOnCallConnectMethod()
        {
            this.ChatClientService.Connect();

            var isChatClientConnected = this.ChatClientService.IsChatClientConnected;

            Assert.True(isChatClientConnected, "The Chat Client must be connected to Chat Server after call Connect method!");
        }

        [Test]
        public void ShouldDisconnectChatClientFromChatServerOnCallDisconnectMethod()
        {
            this.ChatClientService.Connect();
            var isChatClientConnected = this.ChatClientService.IsChatClientConnected;

            this.ChatClientService.Disconnect();
            isChatClientConnected = this.ChatClientService.IsChatClientConnected;

            Assert.False(isChatClientConnected, "The Chat Client must be diconnected from Chat Server after call Disonnect method!");
        }
    }
}
