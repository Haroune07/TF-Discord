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
}