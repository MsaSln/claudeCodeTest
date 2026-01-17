using System.ComponentModel.DataAnnotations;

namespace JwtBackendApi.Models.DTOs;

public class PermissionDto
{
    public int Id { get; set; }
    public int UserGroupId { get; set; }
    public string UserGroupName { get; set; } = string.Empty;
    public int ScreenId { get; set; }
    public string ScreenName { get; set; } = string.Empty;
    public int ScreenActionId { get; set; }
    public string ActionName { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AssignPermissionDto
{
    [Required(ErrorMessage = "User group ID is required")]
    public int UserGroupId { get; set; }

    [Required(ErrorMessage = "Screen ID is required")]
    public int ScreenId { get; set; }

    [Required(ErrorMessage = "At least one action must be specified")]
    [MinLength(1, ErrorMessage = "At least one action must be specified")]
    public List<int> ScreenActionIds { get; set; } = new();

    public bool IsGranted { get; set; } = true;
}

public class RemovePermissionDto
{
    [Required(ErrorMessage = "User group ID is required")]
    public int UserGroupId { get; set; }

    [Required(ErrorMessage = "Screen ID is required")]
    public int ScreenId { get; set; }

    public List<int>? ScreenActionIds { get; set; }
}

public class UserGroupMembershipDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int UserGroupId { get; set; }
    public string UserGroupName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
}

public class AssignUserToGroupDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "At least one user group must be specified")]
    [MinLength(1, ErrorMessage = "At least one user group must be specified")]
    public List<int> UserGroupIds { get; set; } = new();
}

public class UserPermissionCheckDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Screen name is required")]
    public string ScreenName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Action name is required")]
    public string ActionName { get; set; } = string.Empty;
}

public class UserPermissionCheckResult
{
    public bool HasPermission { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> UserGroups { get; set; } = new();
}

public class UserScreenPermissionsDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public List<ScreenPermissionSummary> Screens { get; set; } = new();
}

public class ScreenPermissionSummary
{
    public int ScreenId { get; set; }
    public string ScreenName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Route { get; set; }
    public string? Icon { get; set; }
    public List<string> AllowedActions { get; set; } = new();
}
