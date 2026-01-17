using JwtBackendApi.Models;
using JwtBackendApi.Models.DTOs;

namespace JwtBackendApi.Services;

public class UserGroupService : IUserGroupService
{
    private readonly List<UserGroup> _userGroups;
    private readonly List<UserGroupMembership> _memberships;

    public UserGroupService()
    {
        _userGroups = new List<UserGroup>();
        _memberships = new List<UserGroupMembership>();

        // Seed default groups
        SeedDefaultGroups();
    }

    private void SeedDefaultGroups()
    {
        _userGroups.AddRange(new[]
        {
            new UserGroup
            {
                Id = 1,
                Name = "Administrators",
                Description = "Full system access",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new UserGroup
            {
                Id = 2,
                Name = "Managers",
                Description = "Management level access",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new UserGroup
            {
                Id = 3,
                Name = "Users",
                Description = "Standard user access",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        });
    }

    public async Task<List<UserGroupDto>> GetAllUserGroupsAsync()
    {
        return await Task.FromResult(_userGroups.Select(MapToDto).ToList());
    }

    public async Task<UserGroupDto?> GetUserGroupByIdAsync(int id)
    {
        var group = _userGroups.FirstOrDefault(g => g.Id == id);
        return await Task.FromResult(group != null ? MapToDto(group) : null);
    }

    public async Task<UserGroupDto> CreateUserGroupAsync(CreateUserGroupDto dto)
    {
        var userGroup = new UserGroup
        {
            Id = _userGroups.Count > 0 ? _userGroups.Max(g => g.Id) + 1 : 1,
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _userGroups.Add(userGroup);
        return await Task.FromResult(MapToDto(userGroup));
    }

    public async Task<UserGroupDto?> UpdateUserGroupAsync(int id, UpdateUserGroupDto dto)
    {
        var userGroup = _userGroups.FirstOrDefault(g => g.Id == id);
        if (userGroup == null)
            return null;

        userGroup.Name = dto.Name;
        userGroup.Description = dto.Description;
        userGroup.IsActive = dto.IsActive;
        userGroup.UpdatedAt = DateTime.UtcNow;

        return await Task.FromResult(MapToDto(userGroup));
    }

    public async Task<bool> DeleteUserGroupAsync(int id)
    {
        var userGroup = _userGroups.FirstOrDefault(g => g.Id == id);
        if (userGroup == null)
            return false;

        _userGroups.Remove(userGroup);

        // Remove all memberships
        _memberships.RemoveAll(m => m.UserGroupId == id);

        return await Task.FromResult(true);
    }

    public async Task<List<UserGroupDto>> GetUserGroupsByUserIdAsync(int userId)
    {
        var userGroupIds = _memberships
            .Where(m => m.UserId == userId)
            .Select(m => m.UserGroupId)
            .ToList();

        var groups = _userGroups
            .Where(g => userGroupIds.Contains(g.Id))
            .Select(MapToDto)
            .ToList();

        return await Task.FromResult(groups);
    }

    private UserGroupDto MapToDto(UserGroup group)
    {
        return new UserGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            IsActive = group.IsActive,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt
        };
    }
}
