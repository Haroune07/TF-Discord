using Backend.Src.Models;
using Backend.Src.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.DTOs.Requests;

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
        public async Task<ActionResult<MessageDTO>> SendMessage(CreateMessageRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Content))
                return BadRequest("Content is required.");

            if (string.IsNullOrWhiteSpace(req.ChannelId))
                return BadRequest("ChannelId is required.");

            if (string.IsNullOrWhiteSpace(req.SenderId))
                return BadRequest("SenderId is required.");

            var message = await _messageService.SendMessageAsync(req);
            return Ok(message);
        }

        [HttpGet("channel/{channelId}")]
        public async Task<ActionResult<List<MessageDTO>>> GetMessagesByChannel(string channelId)
        {
            var messages = await _messageService.GetMessagesByChannelAsync(channelId);
            return Ok(messages);
        }

        [HttpPatch("{messageId}")]
        public async Task<ActionResult<MessageDTO>> EditMessage(string messageId, EditMessageRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.NewContent))
                return BadRequest("Content is required.");

            var result = await _messageService.EditMessageAsync(messageId, req.RequesterId, req.NewContent);
            return result != null ? Ok(result) : Forbid();
        }

        [HttpDelete("{messageId}")]
        public async Task<ActionResult> DeleteMessage(string messageId, [FromBody] DeleteMessageRequest req)
        {
            var success = await _messageService.DeleteMessageAsync(messageId, req.RequesterId);
            return success ? Ok() : Forbid();
        }
    }
}
