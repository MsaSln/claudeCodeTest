using JwtBackendApi.Models;
using JwtBackendApi.Models.DTOs;

namespace JwtBackendApi.Services;

public class MenuService : IMenuService
{
    private readonly List<Menu> _menus;
    private readonly List<MenuItem> _menuItems;
    private readonly IScreenService _screenService;
    private readonly IPermissionService _permissionService;

    public MenuService(IScreenService screenService, IPermissionService permissionService)
    {
        _menus = new List<Menu>();
        _menuItems = new List<MenuItem>();
        _screenService = screenService;
        _permissionService = permissionService;

        // Seed default data
        SeedDefaultData();
    }

    private void SeedDefaultData()
    {
        // Create default menus
        _menus.AddRange(new[]
        {
            new Menu
            {
                Id = 1,
                Name = "MainSidebar",
                DisplayName = "Ana Menü",
                Description = "Sol taraf ana menü",
                Location = MenuLocation.Sidebar,
                Order = 1,
                IsActive = true
            },
            new Menu
            {
                Id = 2,
                Name = "TopHeader",
                DisplayName = "Üst Menü",
                Description = "Header menüsü",
                Location = MenuLocation.Header,
                Order = 1,
                IsActive = true
            }
        });

        // Create default menu items
        _menuItems.AddRange(new[]
        {
            // Sidebar Menu Items
            new MenuItem
            {
                Id = 1,
                MenuId = 1,
                ItemType = MenuItemType.Screen,
                ScreenId = 1,
                DisplayName = "Ana Sayfa",
                Icon = "dashboard",
                Route = "/dashboard",
                Order = 1,
                IsActive = true
            },
            new MenuItem
            {
                Id = 2,
                MenuId = 1,
                ItemType = MenuItemType.Group,
                DisplayName = "Yönetim",
                Icon = "settings",
                Order = 2,
                IsActive = true
            },
            new MenuItem
            {
                Id = 3,
                MenuId = 1,
                ParentMenuItemId = 2,
                ItemType = MenuItemType.Screen,
                ScreenId = 2,
                DisplayName = "Kullanıcılar",
                Icon = "people",
                Route = "/users",
                Order = 1,
                IsActive = true
            },
            new MenuItem
            {
                Id = 4,
                MenuId = 1,
                ParentMenuItemId = 2,
                ItemType = MenuItemType.Screen,
                ScreenId = 3,
                DisplayName = "Ayarlar",
                Icon = "settings",
                Route = "/settings",
                Order = 2,
                IsActive = true
            },
            // Header Menu Items
            new MenuItem
            {
                Id = 5,
                MenuId = 2,
                ItemType = MenuItemType.Screen,
                ScreenId = 1,
                DisplayName = "Dashboard",
                Icon = "dashboard",
                Route = "/dashboard",
                Order = 1,
                IsActive = true
            }
        });
    }

    #region Menu Operations

    public async Task<List<MenuDto>> GetAllMenusAsync()
    {
        var menuDtos = new List<MenuDto>();

        foreach (var menu in _menus.OrderBy(m => m.Order))
        {
            var dto = await MapToMenuDto(menu, includeItems: true);
            menuDtos.Add(dto);
        }

        return menuDtos;
    }

    public async Task<MenuDto?> GetMenuByIdAsync(int id)
    {
        var menu = _menus.FirstOrDefault(m => m.Id == id);
        if (menu == null)
            return null;

        return await MapToMenuDto(menu, includeItems: true);
    }

    public async Task<MenuDto> CreateMenuAsync(CreateMenuDto dto, int? createdBy = null)
    {
        var menu = new Menu
        {
            Id = _menus.Count > 0 ? _menus.Max(m => m.Id) + 1 : 1,
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Description = dto.Description,
            Location = dto.Location,
            Order = dto.Order,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        _menus.Add(menu);
        return await MapToMenuDto(menu, includeItems: false);
    }

    public async Task<MenuDto?> UpdateMenuAsync(int id, UpdateMenuDto dto, int? updatedBy = null)
    {
        var menu = _menus.FirstOrDefault(m => m.Id == id);
        if (menu == null)
            return null;

        menu.Name = dto.Name;
        menu.DisplayName = dto.DisplayName;
        menu.Description = dto.Description;
        menu.Location = dto.Location;
        menu.Order = dto.Order;
        menu.IsActive = dto.IsActive;
        menu.UpdatedAt = DateTime.UtcNow;
        menu.UpdatedBy = updatedBy;

        return await MapToMenuDto(menu, includeItems: true);
    }

    public async Task<bool> DeleteMenuAsync(int id)
    {
        var menu = _menus.FirstOrDefault(m => m.Id == id);
        if (menu == null)
            return false;

        _menus.Remove(menu);

        // Remove all menu items belonging to this menu
        _menuItems.RemoveAll(mi => mi.MenuId == id);

        return await Task.FromResult(true);
    }

    #endregion

    #region MenuItem Operations

    public async Task<MenuItemDto?> GetMenuItemByIdAsync(int id)
    {
        var menuItem = _menuItems.FirstOrDefault(mi => mi.Id == id);
        if (menuItem == null)
            return null;

        return await MapToMenuItemDto(menuItem);
    }

    public async Task<List<MenuItemDto>> GetMenuItemsByMenuIdAsync(int menuId)
    {
        var menuItems = _menuItems
            .Where(mi => mi.MenuId == menuId)
            .OrderBy(mi => mi.Order);

        var menuItemDtos = new List<MenuItemDto>();
        foreach (var item in menuItems)
        {
            var dto = await MapToMenuItemDto(item);
            menuItemDtos.Add(dto);
        }

        // Build hierarchical structure
        return BuildHierarchy(menuItemDtos);
    }

    public async Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto, int? createdBy = null)
    {
        // Validate
        if (dto.ItemType == MenuItemType.Screen && !dto.ScreenId.HasValue)
            throw new ArgumentException("ScreenId is required when ItemType is Screen");

        var menuItem = new MenuItem
        {
            Id = _menuItems.Count > 0 ? _menuItems.Max(mi => mi.Id) + 1 : 1,
            MenuId = dto.MenuId,
            ParentMenuItemId = dto.ParentMenuItemId,
            ItemType = dto.ItemType,
            ScreenId = dto.ScreenId,
            DisplayName = dto.DisplayName,
            Icon = dto.Icon,
            Route = dto.Route,
            Order = dto.Order,
            IsActive = dto.IsActive,
            BadgeText = dto.BadgeText,
            BadgeColor = dto.BadgeColor,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        _menuItems.Add(menuItem);
        return await MapToMenuItemDto(menuItem);
    }

    public async Task<MenuItemDto?> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto, int? updatedBy = null)
    {
        var menuItem = _menuItems.FirstOrDefault(mi => mi.Id == id);
        if (menuItem == null)
            return null;

        // Validate
        if (dto.ItemType == MenuItemType.Screen && !dto.ScreenId.HasValue)
            throw new ArgumentException("ScreenId is required when ItemType is Screen");

        menuItem.ParentMenuItemId = dto.ParentMenuItemId;
        menuItem.ItemType = dto.ItemType;
        menuItem.ScreenId = dto.ScreenId;
        menuItem.DisplayName = dto.DisplayName;
        menuItem.Icon = dto.Icon;
        menuItem.Route = dto.Route;
        menuItem.Order = dto.Order;
        menuItem.IsActive = dto.IsActive;
        menuItem.BadgeText = dto.BadgeText;
        menuItem.BadgeColor = dto.BadgeColor;
        menuItem.UpdatedAt = DateTime.UtcNow;
        menuItem.UpdatedBy = updatedBy;

        return await MapToMenuItemDto(menuItem);
    }

    public async Task<bool> DeleteMenuItemAsync(int id)
    {
        var menuItem = _menuItems.FirstOrDefault(mi => mi.Id == id);
        if (menuItem == null)
            return false;

        // Remove the item and all its children
        RemoveMenuItemWithChildren(id);

        return await Task.FromResult(true);
    }

    private void RemoveMenuItemWithChildren(int menuItemId)
    {
        var children = _menuItems.Where(mi => mi.ParentMenuItemId == menuItemId).ToList();

        foreach (var child in children)
        {
            RemoveMenuItemWithChildren(child.Id);
        }

        _menuItems.RemoveAll(mi => mi.Id == menuItemId);
    }

    #endregion

    #region User-specific Menu Operations

    public async Task<UserMenusDto> GetUserMenusAsync(int userId)
    {
        var result = new UserMenusDto();

        foreach (var menu in _menus.Where(m => m.IsActive).OrderBy(m => m.Order))
        {
            var menuItems = await GetMenuItemsWithPermissionsAsync(menu.Id, userId);

            // Only include menu if user has permission to at least one item
            if (menuItems.Any())
            {
                var userMenu = new UserMenuDto
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    DisplayName = menu.DisplayName,
                    Location = menu.Location,
                    Order = menu.Order,
                    Items = menuItems
                };

                if (menu.Location == MenuLocation.Sidebar)
                    result.SidebarMenus.Add(userMenu);
                else
                    result.HeaderMenus.Add(userMenu);
            }
        }

        return result;
    }

    public async Task<List<MenuItemDto>> GetMenuItemsWithPermissionsAsync(int menuId, int userId)
    {
        var menuItems = _menuItems
            .Where(mi => mi.MenuId == menuId && mi.IsActive)
            .OrderBy(mi => mi.Order)
            .ToList();

        var menuItemDtos = new List<MenuItemDto>();

        foreach (var item in menuItems)
        {
            var dto = await MapToMenuItemDto(item);

            // Check permission
            if (item.ItemType == MenuItemType.Screen && item.ScreenId.HasValue)
            {
                var screen = await _screenService.GetScreenByIdAsync(item.ScreenId.Value);
                if (screen != null)
                {
                    // Check if user has at least "view" permission for this screen
                    var hasPermission = await _permissionService.HasPermissionAsync(
                        userId,
                        screen.Name,
                        "view");

                    dto.HasPermission = hasPermission;

                    // Only include if user has permission
                    if (hasPermission)
                        menuItemDtos.Add(dto);
                }
            }
            else if (item.ItemType == MenuItemType.Group)
            {
                // For groups, always add (but children will be filtered)
                dto.HasPermission = true;
                menuItemDtos.Add(dto);
            }
        }

        // Build hierarchy
        var hierarchy = BuildHierarchy(menuItemDtos);

        // Remove empty groups (groups with no visible children)
        return RemoveEmptyGroups(hierarchy);
    }

    private List<MenuItemDto> RemoveEmptyGroups(List<MenuItemDto> items)
    {
        var result = new List<MenuItemDto>();

        foreach (var item in items)
        {
            if (item.ItemType == MenuItemType.Group)
            {
                // Recursively clean children
                item.Children = RemoveEmptyGroups(item.Children);

                // Only include group if it has children
                if (item.Children.Any())
                    result.Add(item);
            }
            else
            {
                result.Add(item);
            }
        }

        return result;
    }

    #endregion

    #region Helper Methods

    private async Task<MenuDto> MapToMenuDto(Menu menu, bool includeItems = false)
    {
        var dto = new MenuDto
        {
            Id = menu.Id,
            Name = menu.Name,
            DisplayName = menu.DisplayName,
            Description = menu.Description,
            Location = menu.Location,
            LocationDisplay = menu.Location.ToString(),
            Order = menu.Order,
            IsActive = menu.IsActive,
            CreatedAt = menu.CreatedAt
        };

        if (includeItems)
        {
            dto.Items = await GetMenuItemsByMenuIdAsync(menu.Id);
        }

        return dto;
    }

    private async Task<MenuItemDto> MapToMenuItemDto(MenuItem menuItem)
    {
        var dto = new MenuItemDto
        {
            Id = menuItem.Id,
            MenuId = menuItem.MenuId,
            ParentMenuItemId = menuItem.ParentMenuItemId,
            ItemType = menuItem.ItemType,
            ItemTypeDisplay = menuItem.ItemType.ToString(),
            ScreenId = menuItem.ScreenId,
            DisplayName = menuItem.DisplayName,
            Icon = menuItem.Icon,
            Route = menuItem.Route,
            Order = menuItem.Order,
            IsActive = menuItem.IsActive,
            BadgeText = menuItem.BadgeText,
            BadgeColor = menuItem.BadgeColor
        };

        // Get screen info if applicable
        if (menuItem.ScreenId.HasValue)
        {
            var screen = await _screenService.GetScreenByIdAsync(menuItem.ScreenId.Value);
            if (screen != null)
            {
                dto.ScreenName = screen.Name;
                // Use screen route if menu item route is not specified
                if (string.IsNullOrEmpty(dto.Route))
                    dto.Route = screen.Route;
                // Use screen icon if menu item icon is not specified
                if (string.IsNullOrEmpty(dto.Icon))
                    dto.Icon = screen.Icon;
            }
        }

        return dto;
    }

    private List<MenuItemDto> BuildHierarchy(List<MenuItemDto> allItems)
    {
        var lookup = allItems.ToDictionary(x => x.Id);
        var rootItems = new List<MenuItemDto>();

        foreach (var item in allItems)
        {
            if (item.ParentMenuItemId.HasValue && lookup.ContainsKey(item.ParentMenuItemId.Value))
            {
                var parent = lookup[item.ParentMenuItemId.Value];
                parent.Children.Add(item);
            }
            else
            {
                rootItems.Add(item);
            }
        }

        // Sort children recursively
        SortChildren(rootItems);

        return rootItems;
    }

    private void SortChildren(List<MenuItemDto> items)
    {
        foreach (var item in items)
        {
            if (item.Children.Any())
            {
                item.Children = item.Children.OrderBy(c => c.Order).ToList();
                SortChildren(item.Children);
            }
        }
    }

    #endregion
}
