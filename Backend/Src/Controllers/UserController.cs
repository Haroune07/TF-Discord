using Backend.Src.Models;
using Backend.Src.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("all/{userId}")]
    public async Task<ActionResult<List<UserDTO>>> GetAllUsers(string userId)
    {
        var users = await _userService.GetAllUsersExceptAsync(userId);
        return Ok(users);
    }

    // endpoint to search users by username
    [HttpGet("search")]
    public async Task<ActionResult<List<UserDTO>>> Search(string username)
    {
        var users = await _userService.SearchUsersAsync(username);
        return Ok(users);
    }

    // endpoint to get user by id
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetById(string id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    // endpoint to update user online status (legacy)
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromQuery] bool isOnline)
    {
        var updated = await _userService.UpdateOnlineStatusAsync(id, isOnline.ToString());
        if (!updated)
            return NotFound();

        return Ok();
    }

    [HttpPut("{userId}/update-pfp")]
    public async Task<IActionResult> UpdateUserProfilePicture(string userId, [FromBody] string newPfpUrl)
    {
        var success = await _userService.UpdatePfpAsync(userId, newPfpUrl);
        if (!success) return NotFound("User not found");
        return NoContent();
    }

    [HttpPut("{userId}/update-status")]
    public async Task<IActionResult> UpdateUserStatus(string userId, [FromBody] string newStatus)
    {
        var success = await _userService.UpdateOnlineStatusAsync(userId, newStatus);
        if (!success) return NotFound("User not found");
        return NoContent();
    }

    [HttpGet("search/{username}")]
    public async Task<ActionResult<List<UserDTO>>> SearchUsers(string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
                return Ok(new List<UserDTO>());
            var results = await _userService.SearchUsersAsync(username);

            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur interne : {ex.Message}");
        }
    }
}
