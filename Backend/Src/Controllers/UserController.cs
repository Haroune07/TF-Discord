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
    // endpoint to update user online status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromQuery] bool isOnline)
    {
        var updated = await _userService.UpdateOnlineStatusAsync(id, isOnline);
        if (!updated)
            return NotFound();

        return Ok();
    }
    



}