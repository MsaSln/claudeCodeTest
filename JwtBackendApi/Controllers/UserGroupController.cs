using JwtBackendApi.Models.DTOs;
using JwtBackendApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtBackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserGroupController : ControllerBase
{
    private readonly IUserGroupService _userGroupService;
    private readonly ILogger<UserGroupController> _logger;

    public UserGroupController(
        IUserGroupService userGroupService,
        ILogger<UserGroupController> logger)
    {
        _userGroupService = userGroupService;
        _logger = logger;
    }

    /// <summary>
    /// Get all user groups
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<UserGroupDto>>> GetAllUserGroups()
    {
        var userGroups = await _userGroupService.GetAllUserGroupsAsync();
        return Ok(userGroups);
    }

    /// <summary>
    /// Get user group by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserGroupDto>> GetUserGroupById(int id)
    {
        var userGroup = await _userGroupService.GetUserGroupByIdAsync(id);
        if (userGroup == null)
            return NotFound(new { message = "User group not found" });

        return Ok(userGroup);
    }

    /// <summary>
    /// Create a new user group
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserGroupDto>> CreateUserGroup([FromBody] CreateUserGroupDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userGroup = await _userGroupService.CreateUserGroupAsync(dto);

        _logger.LogInformation("User group created: {GroupName}", userGroup.Name);

        return CreatedAtAction(
            nameof(GetUserGroupById),
            new { id = userGroup.Id },
            userGroup);
    }

    /// <summary>
    /// Update an existing user group
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserGroupDto>> UpdateUserGroup(int id, [FromBody] UpdateUserGroupDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userGroup = await _userGroupService.UpdateUserGroupAsync(id, dto);
        if (userGroup == null)
            return NotFound(new { message = "User group not found" });

        _logger.LogInformation("User group updated: {GroupId}", id);

        return Ok(userGroup);
    }

    /// <summary>
    /// Delete a user group
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteUserGroup(int id)
    {
        var result = await _userGroupService.DeleteUserGroupAsync(id);
        if (!result)
            return NotFound(new { message = "User group not found" });

        _logger.LogInformation("User group deleted: {GroupId}", id);

        return Ok(new { message = "User group deleted successfully" });
    }

    /// <summary>
    /// Get user groups by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<UserGroupDto>>> GetUserGroupsByUserId(int userId)
    {
        var userGroups = await _userGroupService.GetUserGroupsByUserIdAsync(userId);
        return Ok(userGroups);
    }
}
