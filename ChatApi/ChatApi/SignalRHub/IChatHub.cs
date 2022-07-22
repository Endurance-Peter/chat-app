using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.SignalRHub
{
    public interface IChatHub
    {
        Task ReceiveMessage(string user, string message);
        Task UserConnected(string connectionId);
        Task ReceiveFileMessage(FileMessage fileMessage);
        Task UserDisconnected(string connectionId);
        Task ReceiveMessageToUser(string user, string message);
        Task ReceiveMessageFromGroup(string user, string message);
    }

    public interface IChatHubConnection
    {
        string GetConnectionId();
    }
    
}
