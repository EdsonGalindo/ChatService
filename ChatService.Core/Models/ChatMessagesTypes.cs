using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Core.Models
{
    public static class ChatMessagesTypes
    {
        public enum MessageType
        {
            AdviceNewChatUser = 1,
            MessageToAll = 2,
            MessageFromTo = 3,
            ExitChat = 4
        }
    }
}
