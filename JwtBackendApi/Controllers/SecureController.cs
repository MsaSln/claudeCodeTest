using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JwtBackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SecureController : ControllerBase
{
    private readonly ILogger<SecureController> _logger;

    public SecureController(ILogger<SecureController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get current user profile (requires authentication)
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<object> GetProfile()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Ok(new
        {
            UserId = userId,
            Username = username,
            Email = email,
            Role = role,
            Message = "This is a secure endpoint. You are authenticated!"
        });
    }

    /// <summary>
    /// Get protected data (requires authentication)
    /// </summary>
    /// <returns>Protected data</returns>
    [HttpGet("data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<object> GetSecureData()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        _logger.LogInformation("User {Username} accessed secure data", username);

        return Ok(new
        {
            Message = "This is protected data",
            Data = new[]
            {
                new { Id = 1, Title = "Secure Item 1", Description = "This data requires authentication" },
                new { Id = 2, Title = "Secure Item 2", Description = "Only authenticated users can see this" },
                new { Id = 3, Title = "Secure Item 3", Description = "JWT token is working properly" }
            },
            AccessedBy = username,
            AccessedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Admin only endpoint (requires Admin role)
    /// </summary>
    /// <returns>Admin data</returns>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<object> GetAdminData()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        _logger.LogInformation("Admin user {Username} accessed admin data", username);

        return Ok(new
        {
            Message = "This is admin-only data",
            AdminInfo = "Only users with Admin role can access this endpoint",
            AccessedBy = username
        });
    }
}
