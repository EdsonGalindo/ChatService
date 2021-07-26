using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Core.Interfaces
{
    public interface IChatClientService
    {
        Task Connect();
        void Disconnect();
        void AddMessageSenderWorker();
        void MessageSender();
        void AddMessageReceiverWorker();
        void MessageReceiverWorker();
    }
}
