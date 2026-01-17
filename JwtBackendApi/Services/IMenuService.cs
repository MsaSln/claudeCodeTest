using JwtBackendApi.Models.DTOs;

namespace JwtBackendApi.Services;

public interface IMenuService
{
    // Menu Operations
    Task<List<MenuDto>> GetAllMenusAsync();
    Task<MenuDto?> GetMenuByIdAsync(int id);
    Task<MenuDto> CreateMenuAsync(CreateMenuDto dto, int? createdBy = null);
    Task<MenuDto?> UpdateMenuAsync(int id, UpdateMenuDto dto, int? updatedBy = null);
    Task<bool> DeleteMenuAsync(int id);

    // MenuItem Operations
    Task<MenuItemDto?> GetMenuItemByIdAsync(int id);
    Task<List<MenuItemDto>> GetMenuItemsByMenuIdAsync(int menuId);
    Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto, int? createdBy = null);
    Task<MenuItemDto?> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto, int? updatedBy = null);
    Task<bool> DeleteMenuItemAsync(int id);

    // User-specific Menu Operations
    Task<UserMenusDto> GetUserMenusAsync(int userId);
    Task<List<MenuItemDto>> GetMenuItemsWithPermissionsAsync(int menuId, int userId);
}
