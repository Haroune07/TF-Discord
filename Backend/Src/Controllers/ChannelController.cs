using Backend.Src.Models;
using Backend.Src.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Requests;
using System.Threading.Tasks;

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
        public async Task<ActionResult<Channel>> CreateServerChannel(CreateChannelRequest req)
        {
            var channel = await _channelService.CreateServerChannelAsync(req);
            return Ok(channel);
        }

        [HttpPost("dm")]
        public async Task<ActionResult<Channel>> CreateDMChannel(CreateDMRequest req)
        {
            var channel = await _channelService.CreateDMChannelAsync(req);
            return Ok(channel);
        }
    }
}
