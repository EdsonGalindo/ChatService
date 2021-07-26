using ChatService.Core.Services;
using NUnit.Framework;
using System.Net;

namespace ChatService.Core.Test
{
    public class ChatServerServiceTest
    {
        ChatServerService ChatServerService;

        [SetUp]
        public void Setup()
        {
            this.ChatServerService = new ChatServerService();
            this.ChatServerService.IP = IPAddress.Parse("127.0.0.1");
            this.ChatServerService.Port = 8182;
        }

        [Test]
        public void ShouldIniateChatServerOnCallStartMethod()
        {
            this.ChatServerService.Start();

            var chatServerStatus = this.ChatServerService.IsChatServerActive;

            Assert.True(chatServerStatus, "The Chat Server must be running after call Start method!");
        }

        [Test]
        public void ShouldStopChatServerOnCallStopMethod()
        {
            this.ChatServerService.Start();
            var chatServerStatus = this.ChatServerService.IsChatServerActive;
            this.ChatServerService.Stop();
            chatServerStatus = this.ChatServerService.IsChatServerActive;

            Assert.False(chatServerStatus, "The Chat Server must be stopped after call Stop method!");
        }
    }
}