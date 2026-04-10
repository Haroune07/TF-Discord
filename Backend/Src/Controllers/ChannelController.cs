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
        public async Task<ActionResult<ChannelDTO>> CreateServerChannel(CreateChannelRequest req)
        {
            var channel = await _channelService.CreateServerChannelAsync(req);
            return Ok(channel);
        }

        [HttpPost("dm")]
        public async Task<ActionResult<ChannelDTO>> CreateDMChannel(CreateDMRequest req)
        {
            var channel = await _channelService.CreateDMChannelAsync(req);
            return Ok(channel);
        }

        [HttpGet("server/{serverId}")]
        public async Task<ActionResult<List<ChannelDTO>>> GetServerChannels(string serverId)
        {
            var channels = await _channelService.GetServerChannelsAsync(serverId);
            return Ok(channels);
        }
    }
}