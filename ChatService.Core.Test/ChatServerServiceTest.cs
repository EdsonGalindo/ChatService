using ChatService.Core.Services;
using NUnit.Framework;
using System.Net;
using System.Threading;

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
        }

        [Test]
        public void ShouldIniateChatServerOnCallStartMethod()
        {
            // Changes the Port because the test runs too fast and the port may still be allocated yet
            this.ChatServerService.Port = 8183; 
            this.ChatServerService.Start();

            var chatServerStatus = this.ChatServerService.IsChatServerActive;

            Assert.True(chatServerStatus, "The Chat Server must be running after call Start method!");
        }

        [Test]
        public void ShouldStopChatServerOnCallStopMethod()
        {
            // Changes the Port because the test runs too fast and the port may still be allocated yet
            this.ChatServerService.Port = 8184;
            this.ChatServerService.Start();
            var chatServerStatus = this.ChatServerService.IsChatServerActive;
            this.ChatServerService.Stop();
            chatServerStatus = this.ChatServerService.IsChatServerActive;

            Assert.False(chatServerStatus, "The Chat Server must be stopped after call Stop method!");
        }
    }
}