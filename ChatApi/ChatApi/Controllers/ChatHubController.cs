using ChatApi.SignalRHub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("chat-hub")]
    public class ChatHubController : ControllerBase
    {
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public ChatHubController(IHubContext<ChatHub,IChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        [HttpPost("send-to-all")]
        public async Task<IActionResult> SendMessageToAllConnectedClient(string userName, string message)
        {
            try
            {
                await _hubContext.Clients.All.ReceiveMessage(userName, message);
            }
            catch (Exception ex)
            {

                throw;
            }
            return Ok();
        }
        
        [HttpPost("send-to-user")]
        public async Task<IActionResult> SendMessageToConnectedClient(string userName, string message)
        {
            await _hubContext.Clients.User(userName).ReceiveMessage(userName, message);
            return Ok();
        }
    }
}
