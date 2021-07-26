using ChatService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChatService.Core.Models.ChatMessagesTypes;

namespace ChatService.Core.Models
{
    public class ChatMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public MessageType Type { get; set; } = MessageType.MessageToAll;
    }
}
