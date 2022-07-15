using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.SignalRHub
{
    public interface IChatHub
    {
        Task ReceiveMessage(string user, string message);
        Task ReceiveMessageToUser(string user, string message);
    }
}
