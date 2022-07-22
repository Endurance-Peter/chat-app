using ChatApi.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.SignalRHub
{
    public class ChatHub : Hub<IChatHub>, IChatHubConnection
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }

        public async Task SendMessageToUser(string receiverConnectionId, string user, string message)
        {
            await Clients.Clients(receiverConnectionId).ReceiveMessageToUser(user, message);
        }

        public async Task SendFileMessage(string receiverConnectionId, FileMessage fileMessage)
        {
            await Clients.Client(receiverConnectionId).ReceiveFileMessage(fileMessage);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task RemoveCleintFromGroup(string groupName, string cleintId)
        {
            await Groups.RemoveFromGroupAsync(cleintId, groupName);
        }
        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task SendMessageToGroup(string groupName, string user, string message)
        {
            await Clients.Group(groupName).ReceiveMessageFromGroup(user, message);
        }
        //public override async Task OnConnectedAsync()
        //{
        //    await Clients.All.UserConnected(Context.ConnectionId);
        //    await base.OnConnectedAsync();
        //}
        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    await Clients.Caller.UserDisconnected(Context.ConnectionId);
        //    await base.OnDisconnectedAsync(exception);
        //}
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
