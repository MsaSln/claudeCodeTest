using JwtBackendApi.Models;
using JwtBackendApi.Models.DTOs;

namespace JwtBackendApi.Services;

public class PermissionService : IPermissionService
{
    private readonly List<ScreenPermission> _permissions;
    private readonly List<UserGroupMembership> _memberships;
    private readonly IUserGroupService _userGroupService;
    private readonly IScreenService _screenService;
    private readonly IAuthService _authService;

    public PermissionService(
        IUserGroupService userGroupService,
        IScreenService screenService,
        IAuthService authService)
    {
        _permissions = new List<ScreenPermission>();
        _memberships = new List<UserGroupMembership>();
        _userGroupService = userGroupService;
        _screenService = screenService;
        _authService = authService;

        // Seed default permissions
        SeedDefaultPermissions();
    }

    private void SeedDefaultPermissions()
    {
        // Admin group gets all permissions
        _permissions.AddRange(new[]
        {
            // Dashboard - Admin
            new ScreenPermission { Id = 1, UserGroupId = 1, ScreenId = 1, ScreenActionId = 1, IsGranted = true },
            new ScreenPermission { Id = 2, UserGroupId = 1, ScreenId = 1, ScreenActionId = 2, IsGranted = true },

            // Users - Admin
            new ScreenPermission { Id = 3, UserGroupId = 1, ScreenId = 2, ScreenActionId = 3, IsGranted = true },
            new ScreenPermission { Id = 4, UserGroupId = 1, ScreenId = 2, ScreenActionId = 4, IsGranted = true },
            new ScreenPermission { Id = 5, UserGroupId = 1, ScreenId = 2, ScreenActionId = 5, IsGranted = true },
            new ScreenPermission { Id = 6, UserGroupId = 1, ScreenId = 2, ScreenActionId = 6, IsGranted = true },
            new ScreenPermission { Id = 7, UserGroupId = 1, ScreenId = 2, ScreenActionId = 7, IsGranted = true },

            // Settings - Admin
            new ScreenPermission { Id = 8, UserGroupId = 1, ScreenId = 3, ScreenActionId = 8, IsGranted = true },
            new ScreenPermission { Id = 9, UserGroupId = 1, ScreenId = 3, ScreenActionId = 9, IsGranted = true },

            // Users - Standard Users (view only)
            new ScreenPermission { Id = 10, UserGroupId = 3, ScreenId = 1, ScreenActionId = 1, IsGranted = true },
            new ScreenPermission { Id = 11, UserGroupId = 3, ScreenId = 2, ScreenActionId = 3, IsGranted = true }
        });
    }

    // Permission Management
    public async Task<List<PermissionDto>> GetAllPermissionsAsync()
    {
        var permissionDtos = new List<PermissionDto>();

        foreach (var permission in _permissions)
        {
            var dto = await MapToPermissionDto(permission);
            if (dto != null)
                permissionDtos.Add(dto);
        }

        return permissionDtos;
    }

    public async Task<List<PermissionDto>> GetPermissionsByUserGroupIdAsync(int userGroupId)
    {
        var permissions = _permissions.Where(p => p.UserGroupId == userGroupId);
        var permissionDtos = new List<PermissionDto>();

        foreach (var permission in permissions)
        {
            var dto = await MapToPermissionDto(permission);
            if (dto != null)
                permissionDtos.Add(dto);
        }

        return permissionDtos;
    }

    public async Task<List<PermissionDto>> GetPermissionsByScreenIdAsync(int screenId)
    {
        var permissions = _permissions.Where(p => p.ScreenId == screenId);
        var permissionDtos = new List<PermissionDto>();

        foreach (var permission in permissions)
        {
            var dto = await MapToPermissionDto(permission);
            if (dto != null)
                permissionDtos.Add(dto);
        }

        return permissionDtos;
    }

    public async Task<bool> AssignPermissionsAsync(AssignPermissionDto dto, int? assignedBy = null)
    {
        // Validate user group exists
        var userGroup = await _userGroupService.GetUserGroupByIdAsync(dto.UserGroupId);
        if (userGroup == null)
            return false;

        // Validate screen exists
        var screen = await _screenService.GetScreenByIdAsync(dto.ScreenId);
        if (screen == null)
            return false;

        // Remove existing permissions for this combination
        _permissions.RemoveAll(p =>
            p.UserGroupId == dto.UserGroupId &&
            p.ScreenId == dto.ScreenId &&
            dto.ScreenActionIds.Contains(p.ScreenActionId));

        // Add new permissions
        foreach (var actionId in dto.ScreenActionIds)
        {
            var action = await _screenService.GetScreenActionByIdAsync(actionId);
            if (action == null || action.ScreenId != dto.ScreenId)
                continue;

            var permission = new ScreenPermission
            {
                Id = _permissions.Count > 0 ? _permissions.Max(p => p.Id) + 1 : 1,
                UserGroupId = dto.UserGroupId,
                ScreenId = dto.ScreenId,
                ScreenActionId = actionId,
                IsGranted = dto.IsGranted,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = assignedBy
            };

            _permissions.Add(permission);
        }

        return await Task.FromResult(true);
    }

    public async Task<bool> RemovePermissionsAsync(RemovePermissionDto dto)
    {
        if (dto.ScreenActionIds == null || dto.ScreenActionIds.Count == 0)
        {
            // Remove all permissions for this user group and screen
            _permissions.RemoveAll(p =>
                p.UserGroupId == dto.UserGroupId &&
                p.ScreenId == dto.ScreenId);
        }
        else
        {
            // Remove specific action permissions
            _permissions.RemoveAll(p =>
                p.UserGroupId == dto.UserGroupId &&
                p.ScreenId == dto.ScreenId &&
                dto.ScreenActionIds.Contains(p.ScreenActionId));
        }

        return await Task.FromResult(true);
    }

    // User-Group Membership Management
    public async Task<List<UserGroupMembershipDto>> GetUserGroupMembershipsAsync(int userId)
    {
        var memberships = _memberships.Where(m => m.UserId == userId);
        var membershipDtos = new List<UserGroupMembershipDto>();

        foreach (var membership in memberships)
        {
            var dto = await MapToMembershipDto(membership);
            if (dto != null)
                membershipDtos.Add(dto);
        }

        return membershipDtos;
    }

    public async Task<bool> AssignUserToGroupsAsync(AssignUserToGroupDto dto, int? assignedBy = null)
    {
        // Validate user exists
        var user = await _authService.GetUserByIdAsync(dto.UserId);
        if (user == null)
            return false;

        // Remove existing memberships for this user
        _memberships.RemoveAll(m => m.UserId == dto.UserId);

        // Add new memberships
        foreach (var groupId in dto.UserGroupIds)
        {
            var userGroup = await _userGroupService.GetUserGroupByIdAsync(groupId);
            if (userGroup == null)
                continue;

            var membership = new UserGroupMembership
            {
                Id = _memberships.Count > 0 ? _memberships.Max(m => m.Id) + 1 : 1,
                UserId = dto.UserId,
                UserGroupId = groupId,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = assignedBy
            };

            _memberships.Add(membership);
        }

        return await Task.FromResult(true);
    }

    public async Task<bool> RemoveUserFromGroupAsync(int userId, int userGroupId)
    {
        var membership = _memberships.FirstOrDefault(m =>
            m.UserId == userId && m.UserGroupId == userGroupId);

        if (membership == null)
            return false;

        _memberships.Remove(membership);
        return await Task.FromResult(true);
    }

    // Permission Checking
    public async Task<UserPermissionCheckResult> CheckUserPermissionAsync(UserPermissionCheckDto dto)
    {
        var hasPermission = await HasPermissionAsync(dto.UserId, dto.ScreenName, dto.ActionName);

        var userGroups = await GetUserGroupMembershipsAsync(dto.UserId);
        var groupNames = userGroups.Select(g => g.UserGroupName).ToList();

        return new UserPermissionCheckResult
        {
            HasPermission = hasPermission,
            Message = hasPermission
                ? $"User has permission to {dto.ActionName} on {dto.ScreenName}"
                : $"User does not have permission to {dto.ActionName} on {dto.ScreenName}",
            UserGroups = groupNames
        };
    }

    public async Task<UserScreenPermissionsDto> GetUserScreenPermissionsAsync(int userId)
    {
        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        // Get user's groups
        var userGroupIds = _memberships
            .Where(m => m.UserId == userId)
            .Select(m => m.UserGroupId)
            .ToList();

        // Get all screens
        var allScreens = await _screenService.GetAllScreensAsync();

        var screenPermissions = new List<ScreenPermissionSummary>();

        foreach (var screen in allScreens)
        {
            // Get allowed actions for this screen
            var allowedActionIds = _permissions
                .Where(p => userGroupIds.Contains(p.UserGroupId) &&
                           p.ScreenId == screen.Id &&
                           p.IsGranted)
                .Select(p => p.ScreenActionId)
                .Distinct()
                .ToList();

            if (allowedActionIds.Count == 0)
                continue;

            var allowedActions = new List<string>();
            foreach (var actionId in allowedActionIds)
            {
                var action = await _screenService.GetScreenActionByIdAsync(actionId);
                if (action != null)
                    allowedActions.Add(action.ActionName);
            }

            screenPermissions.Add(new ScreenPermissionSummary
            {
                ScreenId = screen.Id,
                ScreenName = screen.Name,
                DisplayName = screen.DisplayName,
                Route = screen.Route,
                Icon = screen.Icon,
                AllowedActions = allowedActions
            });
        }

        return new UserScreenPermissionsDto
        {
            UserId = userId,
            Username = user.Username,
            Screens = screenPermissions.OrderBy(s => s.ScreenName).ToList()
        };
    }

    public async Task<bool> HasPermissionAsync(int userId, string screenName, string actionName)
    {
        // Get user's groups
        var userGroupIds = _memberships
            .Where(m => m.UserId == userId)
            .Select(m => m.UserGroupId)
            .ToList();

        if (userGroupIds.Count == 0)
            return false;

        // Get screen by name
        var screen = await _screenService.GetScreenByNameAsync(screenName);
        if (screen == null)
            return false;

        // Get action by name
        var action = screen.Actions.FirstOrDefault(a =>
            a.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase));
        if (action == null)
            return false;

        // Check if user has permission
        var hasPermission = _permissions.Any(p =>
            userGroupIds.Contains(p.UserGroupId) &&
            p.ScreenId == screen.Id &&
            p.ScreenActionId == action.Id &&
            p.IsGranted);

        return hasPermission;
    }

    // Mapping methods
    private async Task<PermissionDto?> MapToPermissionDto(ScreenPermission permission)
    {
        var userGroup = await _userGroupService.GetUserGroupByIdAsync(permission.UserGroupId);
        var screen = await _screenService.GetScreenByIdAsync(permission.ScreenId);
        var action = await _screenService.GetScreenActionByIdAsync(permission.ScreenActionId);

        if (userGroup == null || screen == null || action == null)
            return null;

        return new PermissionDto
        {
            Id = permission.Id,
            UserGroupId = permission.UserGroupId,
            UserGroupName = userGroup.Name,
            ScreenId = permission.ScreenId,
            ScreenName = screen.Name,
            ScreenActionId = permission.ScreenActionId,
            ActionName = action.ActionName,
            IsGranted = permission.IsGranted,
            CreatedAt = permission.CreatedAt
        };
    }

    private async Task<UserGroupMembershipDto?> MapToMembershipDto(UserGroupMembership membership)
    {
        var user = await _authService.GetUserByIdAsync(membership.UserId);
        var userGroup = await _userGroupService.GetUserGroupByIdAsync(membership.UserGroupId);

        if (user == null || userGroup == null)
            return null;

        return new UserGroupMembershipDto
        {
            Id = membership.Id,
            UserId = membership.UserId,
            Username = user.Username,
            UserGroupId = membership.UserGroupId,
            UserGroupName = userGroup.Name,
            AssignedAt = membership.AssignedAt
        };
    }
}
