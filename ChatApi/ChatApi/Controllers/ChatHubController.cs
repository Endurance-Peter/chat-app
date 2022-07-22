using ChatApi.SignalRHub;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("chat-hub")]
    public class ChatHubController : ControllerBase
    {
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;
        private readonly IChatHubConnection _chatHubConnection;
        private string userConnectionId;

        public ChatHubController(IHubContext<ChatHub,IChatHub> hubContext, IChatHubConnection chatHubConnection)
        {
            _hubContext = hubContext;
            _chatHubConnection = chatHubConnection;
            //userConnectionId = _chatHubConnection.GetConnectionId();
        }
        [HttpPost("publish-to-all")]
        public async Task<IActionResult> SendMessageToAllConnectedClient(string userName, string message)
        {
            try
            {
                await _hubContext.Clients.All.ReceiveMessage(userName, message);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }
            return Ok();
        }
        
        [HttpPost("send-to-user")]
        public async Task<IActionResult> SendMessageToConnectedClient(string receiverConnectionId, string userName, string message)
        {
            try
            {
                await _hubContext.Clients.Client(receiverConnectionId).ReceiveMessage(userName, message);
            }
            catch (Exception)
            {

                return BadRequest();
            }
            return Ok();
        }

        [HttpPost("send-file")]
        public async Task<IActionResult> SendFileToConnectedClient(string receiverConnectionId, [FromForm] FileDocument choseFile)
        {
            foreach (var file in choseFile.Files)
            {
                if (file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        var fileMessage = new FileMessage
                        {
                            FileHeader = $"data: {file.ContentType} ;base64",
                            FileBinary = memoryStream.ToArray()
                        };

                        try
                        {
                            await _hubContext.Clients.User(receiverConnectionId).ReceiveFileMessage(fileMessage);
                        }
                        catch (Exception)
                        {

                            return BadRequest();
                        }
                    }
                }
            }

            return Ok();
        }

        [HttpPost("join-group")]
        public async Task<IActionResult> JoinGroup(string groupName)
        {
            try
            {
                await _hubContext.Groups.AddToGroupAsync(userConnectionId, groupName);

            }
            catch (Exception)
            {

                return BadRequest();
            }
            return Ok();
        }

        [HttpPut("remove-cleint")]
        public async Task<IActionResult> RemoveCleintFromGroup(string groupName, string cleintId)
        {
            try
            {
                await _hubContext.Groups.RemoveFromGroupAsync(cleintId, groupName);
            }
            catch (Exception)
            {

                return BadRequest();
            }
            return Ok();
        }

        [HttpPut("exit-group")]
        public async Task<IActionResult> RemoveFromGroup(string groupName)
        {
            try
            {
                await _hubContext.Groups.RemoveFromGroupAsync(userConnectionId, groupName);

            }
            catch (Exception)
            {

                return BadRequest();
            }
            return Ok();
        }
        [HttpPost("publish-to-group")]
        public async Task<IActionResult> SendMessageToGroup(string groupName, string user, string message)
        {
            try
            {
                await _hubContext.Clients.Group(groupName).ReceiveMessageFromGroup(user, message);

            }
            catch (Exception)
            {

                return BadRequest();
            }
            return Ok();
        }
        [HttpGet("connectionId")]
        public IActionResult GetConnectionId()
        {
            return Ok(userConnectionId);
        }
    }
}
