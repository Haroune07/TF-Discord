using Backend.Src.Models;
using Backend.Src.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("send")]
        public async Task<ActionResult<Message>> SendMessage(CreateMessageRequest req)
        {
            var message = await _messageService.SendMessageAsync(req);
            return Ok(message);
        }

        [HttpGet("channel/{channelId}")]
        public async Task<ActionResult<List<Message>>> GetMessagesByChannel(string channelId)
        {
            var messages = await _messageService.GetMessagesByChannelAsync(channelId);
            return Ok(messages);
        }
    }
}
