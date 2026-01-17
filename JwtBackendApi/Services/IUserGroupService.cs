using JwtBackendApi.Models;
using JwtBackendApi.Models.DTOs;

namespace JwtBackendApi.Services;

public interface IUserGroupService
{
    Task<List<UserGroupDto>> GetAllUserGroupsAsync();
    Task<UserGroupDto?> GetUserGroupByIdAsync(int id);
    Task<UserGroupDto> CreateUserGroupAsync(CreateUserGroupDto dto);
    Task<UserGroupDto?> UpdateUserGroupAsync(int id, UpdateUserGroupDto dto);
    Task<bool> DeleteUserGroupAsync(int id);
    Task<List<UserGroupDto>> GetUserGroupsByUserIdAsync(int userId);
}
