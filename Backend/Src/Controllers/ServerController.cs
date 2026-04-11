using Backend.Src.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.DTOs.Requests;

namespace Backend.Src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private readonly ServerService _serverService;

        public ServerController(ServerService serverService)
        {
            _serverService = serverService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ServerDTO>> CreateServer(CreateServerRequest req)
        {
            var server = await _serverService.CreateServerAsync(req);
            return Ok(server);
        }

        [HttpGet("mine/{userId}")]
        public async Task<ActionResult<List<ServerDTO>>> GetMyServers(string userId)
        {
            var servers = await _serverService.GetUserServersAsync(userId);
            return Ok(servers);
        }

        [HttpPost("join")]
        public async Task<ActionResult> JoinServer(JoinOrLeaveServerRequest req)
        {
            try 
            {
                await _serverService.JoinServerAsync(req);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("leave")]
        public async Task<ActionResult> LeaveServer(JoinOrLeaveServerRequest req) 
        {
            // Note: On réutilise JoinServerRequest car il contient déjà ServerId et UserId
            try
            {
                await _serverService.LeaveServer(req);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}