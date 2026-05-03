using Backend.Src.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Enums;

namespace Backend.Src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipController : ControllerBase
    {
        private readonly FriendshipService _friendshipService;

        public FriendshipController(FriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> SendRequest([FromQuery] string requesterId, [FromQuery] string targetUsername)
        {
            var success = await _friendshipService.SendFriendRequestAsync(requesterId, targetUsername);
            return success ? Ok() : BadRequest("Could not send friend request.");
        }

        [HttpGet("friends/{userId}")]
        public async Task<ActionResult<List<FriendshipDTO>>> GetFriends(string userId)
        {
            var friends = await _friendshipService.GetFriendsAsync(userId);
            return Ok(friends);
        }

        [HttpGet("pending/{userId}")]
        public async Task<ActionResult<List<FriendshipDTO>>> GetPendingRequests(string userId)
        {
            var pending = await _friendshipService.GetPendingRequestsAsync(userId);
            return Ok(pending);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromQuery] string userId, [FromQuery] FriendshipStatus status)
        {
            var success = await _friendshipService.UpdateFriendshipStatusAsync(id, userId, status);
            return success ? Ok() : BadRequest("Could not update friendship status.");
        }
    }
}
