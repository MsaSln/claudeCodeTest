using JwtBackendApi.Models.DTOs;

namespace JwtBackendApi.Services;

public interface IPermissionService
{
    // Permission management
    Task<List<PermissionDto>> GetAllPermissionsAsync();
    Task<List<PermissionDto>> GetPermissionsByUserGroupIdAsync(int userGroupId);
    Task<List<PermissionDto>> GetPermissionsByScreenIdAsync(int screenId);
    Task<bool> AssignPermissionsAsync(AssignPermissionDto dto, int? assignedBy = null);
    Task<bool> RemovePermissionsAsync(RemovePermissionDto dto);

    // User-Group membership management
    Task<List<UserGroupMembershipDto>> GetUserGroupMembershipsAsync(int userId);
    Task<bool> AssignUserToGroupsAsync(AssignUserToGroupDto dto, int? assignedBy = null);
    Task<bool> RemoveUserFromGroupAsync(int userId, int userGroupId);

    // Permission check
    Task<UserPermissionCheckResult> CheckUserPermissionAsync(UserPermissionCheckDto dto);
    Task<UserScreenPermissionsDto> GetUserScreenPermissionsAsync(int userId);
    Task<bool> HasPermissionAsync(int userId, string screenName, string actionName);
}
