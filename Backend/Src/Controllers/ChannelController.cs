using Backend.Src.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.DTOs.Requests;

namespace Backend.Src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelController : ControllerBase
    {
        private readonly ChannelService _channelService;

        public ChannelController(ChannelService channelService)
        {
            _channelService = channelService;
        }

        [HttpPost("server")]
        public async Task<ActionResult<ChannelDTO>> CreateServerChannel(
            [FromQuery] string requesterId,
            [FromBody] CreateChannelRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return BadRequest("Name is required.");

            if (string.IsNullOrWhiteSpace(req.ServerId))
                return BadRequest("ServerId is required.");

            if (string.IsNullOrWhiteSpace(requesterId))
                return BadRequest("requesterId is required.");

            var channel = await _channelService.CreateChannelAsync(req.ServerId, requesterId, req);
            if (channel == null)
                return Forbid();

            return Ok(channel);
        }

        [HttpDelete("{channelId}")]
        public async Task<IActionResult> DeleteChannel(string channelId, [FromQuery] string requesterId)
        {
            if (string.IsNullOrWhiteSpace(channelId))
                return BadRequest("channelId is required.");

            if (string.IsNullOrWhiteSpace(requesterId))
                return BadRequest("requesterId is required.");

            var deleted = await _channelService.DeleteChannelAsync(channelId, requesterId);
            if (!deleted)
                return Forbid();

            return NoContent();
        }

        [HttpPost("dm")]
        public async Task<ActionResult<ChannelDTO>> CreateDMChannel(CreateDMRequest req)
        {
            var channel = await _channelService.CreateDMChannelAsync(req);
            return Ok(channel);
        }

        [HttpGet("server/{serverId}")]
        public async Task<ActionResult<List<ChannelDTO>>> GetServerChannels(string serverId, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("userId is required.");

            var channels = await _channelService.GetServerChannelsAsync(serverId, userId);
            if (channels == null)
                return Forbid();

            return Ok(channels);
        }

        [HttpGet("dm/{userId}")]
        public async Task<ActionResult<List<ChannelDTO>>> GetDMChannels(string userId)
        {
            var channels = await _channelService.GetUserDMChannelsAsync(userId);
            return Ok(channels);
        }
    }
}
