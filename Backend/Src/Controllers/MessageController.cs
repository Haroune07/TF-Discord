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
            if (string.IsNullOrWhiteSpace(req.Content))
            {
                return BadRequest("Content is required.");
            }

            if (string.IsNullOrWhiteSpace(req.ChannelId))
            {
                return BadRequest("ChannelId is required.");
            }

            if (string.IsNullOrWhiteSpace(req.SenderId))
            {
                return BadRequest("SenderId is required.");
            }

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
