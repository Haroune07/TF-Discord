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

    [HttpPut("{userId}/update-pfp")]
    public async Task<IActionResult> UpdateUserProfilePicture(string userId, [FromBody] string newPfpUrl)
    {
        // On appelle le service pour faire le travail logique
        var success = await _userService.UpdatePfpAsync(userId, newPfpUrl);

        if (!success) return NotFound("Utilisateur non trouvé");

        return NoContent();
    }

    [HttpPut("{userId}/update-status")]
    public async Task<IActionResult> UpdateUserStatus(string userId, [FromBody] string newStatus)
    {
        // On appelle le service pour faire le travail logique
        var success = await _userService.UpdateStatusAsync(userId, newStatus);

        if (!success) return NotFound("Utilisateur non trouvé");

        return NoContent();
    }
}