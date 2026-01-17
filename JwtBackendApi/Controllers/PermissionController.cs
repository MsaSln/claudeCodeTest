using JwtBackendApi.Models.DTOs;
using JwtBackendApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JwtBackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionController> _logger;

    public PermissionController(
        IPermissionService permissionService,
        ILogger<PermissionController> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    #region Permission Management

    /// <summary>
    /// Get all permissions
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<PermissionDto>>> GetAllPermissions()
    {
        var permissions = await _permissionService.GetAllPermissionsAsync();
        return Ok(permissions);
    }

    /// <summary>
    /// Get permissions by user group ID
    /// </summary>
    [HttpGet("usergroup/{userGroupId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<PermissionDto>>> GetPermissionsByUserGroupId(int userGroupId)
    {
        var permissions = await _permissionService.GetPermissionsByUserGroupIdAsync(userGroupId);
        return Ok(permissions);
    }

    /// <summary>
    /// Get permissions by screen ID
    /// </summary>
    [HttpGet("screen/{screenId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<PermissionDto>>> GetPermissionsByScreenId(int screenId)
    {
        var permissions = await _permissionService.GetPermissionsByScreenIdAsync(screenId);
        return Ok(permissions);
    }

    /// <summary>
    /// Assign permissions to a user group for a screen
    /// </summary>
    [HttpPost("assign")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> AssignPermissions([FromBody] AssignPermissionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserId = GetCurrentUserId();
        var result = await _permissionService.AssignPermissionsAsync(dto, currentUserId);

        if (!result)
            return BadRequest(new { message = "Failed to assign permissions. Check if user group and screen exist." });

        _logger.LogInformation(
            "Permissions assigned: UserGroup {UserGroupId}, Screen {ScreenId}, Actions: {ActionIds}",
            dto.UserGroupId, dto.ScreenId, string.Join(",", dto.ScreenActionIds));

        return Ok(new { message = "Permissions assigned successfully" });
    }

    /// <summary>
    /// Remove permissions from a user group for a screen
    /// </summary>
    [HttpPost("remove")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> RemovePermissions([FromBody] RemovePermissionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _permissionService.RemovePermissionsAsync(dto);

        _logger.LogInformation(
            "Permissions removed: UserGroup {UserGroupId}, Screen {ScreenId}",
            dto.UserGroupId, dto.ScreenId);

        return Ok(new { message = "Permissions removed successfully" });
    }

    #endregion

    #region User-Group Membership Management

    /// <summary>
    /// Get user group memberships for a specific user
    /// </summary>
    [HttpGet("user/{userId}/groups")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<UserGroupMembershipDto>>> GetUserGroupMemberships(int userId)
    {
        var memberships = await _permissionService.GetUserGroupMembershipsAsync(userId);
        return Ok(memberships);
    }

    /// <summary>
    /// Assign user to user groups
    /// </summary>
    [HttpPost("user/assign-groups")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> AssignUserToGroups([FromBody] AssignUserToGroupDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserId = GetCurrentUserId();
        var result = await _permissionService.AssignUserToGroupsAsync(dto, currentUserId);

        if (!result)
            return BadRequest(new { message = "Failed to assign user to groups. Check if user and groups exist." });

        _logger.LogInformation(
            "User {UserId} assigned to groups: {GroupIds}",
            dto.UserId, string.Join(",", dto.UserGroupIds));

        return Ok(new { message = "User assigned to groups successfully" });
    }

    /// <summary>
    /// Remove user from a user group
    /// </summary>
    [HttpDelete("user/{userId}/group/{userGroupId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> RemoveUserFromGroup(int userId, int userGroupId)
    {
        var result = await _permissionService.RemoveUserFromGroupAsync(userId, userGroupId);

        if (!result)
            return NotFound(new { message = "User group membership not found" });

        _logger.LogInformation("User {UserId} removed from group {GroupId}", userId, userGroupId);

        return Ok(new { message = "User removed from group successfully" });
    }

    #endregion

    #region Permission Checking

    /// <summary>
    /// Check if a user has permission for a specific screen action
    /// </summary>
    [HttpPost("check")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserPermissionCheckResult>> CheckUserPermission([FromBody] UserPermissionCheckDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _permissionService.CheckUserPermissionAsync(dto);
        return Ok(result);
    }

    /// <summary>
    /// Get all screen permissions for a user
    /// </summary>
    [HttpGet("user/{userId}/screens")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserScreenPermissionsDto>> GetUserScreenPermissions(int userId)
    {
        try
        {
            var permissions = await _permissionService.GetUserScreenPermissionsAsync(userId);
            return Ok(permissions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get current user's screen permissions
    /// </summary>
    [HttpGet("my-permissions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserScreenPermissionsDto>> GetMyPermissions()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new { message = "User not authenticated" });

        try
        {
            var permissions = await _permissionService.GetUserScreenPermissionsAsync(userId.Value);
            return Ok(permissions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Check if current user has permission for a specific screen action
    /// </summary>
    [HttpGet("can-access/{screenName}/{actionName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<object>> CanAccess(string screenName, string actionName)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new { message = "User not authenticated" });

        var hasPermission = await _permissionService.HasPermissionAsync(userId.Value, screenName, actionName);

        return Ok(new
        {
            screenName,
            actionName,
            hasPermission,
            message = hasPermission
                ? "Access granted"
                : "Access denied"
        });
    }

    #endregion

    #region Helper Methods

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
            return userId;
        return null;
    }

    #endregion
}
