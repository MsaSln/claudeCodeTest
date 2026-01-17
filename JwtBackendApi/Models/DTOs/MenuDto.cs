using System.ComponentModel.DataAnnotations;

namespace JwtBackendApi.Models.DTOs;

public class MenuDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MenuLocation Location { get; set; }
    public string LocationDisplay { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public List<MenuItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateMenuDto
{
    [Required(ErrorMessage = "Menu name is required")]
    [StringLength(100, ErrorMessage = "Menu name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display name is required")]
    [StringLength(200, ErrorMessage = "Display name cannot exceed 200 characters")]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Location is required")]
    public MenuLocation Location { get; set; }

    [Range(0, 9999, ErrorMessage = "Order must be between 0 and 9999")]
    public int Order { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}

public class UpdateMenuDto
{
    [Required(ErrorMessage = "Menu name is required")]
    [StringLength(100, ErrorMessage = "Menu name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display name is required")]
    [StringLength(200, ErrorMessage = "Display name cannot exceed 200 characters")]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Location is required")]
    public MenuLocation Location { get; set; }

    [Range(0, 9999, ErrorMessage = "Order must be between 0 and 9999")]
    public int Order { get; set; }

    public bool IsActive { get; set; }
}

public class MenuItemDto
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public int? ParentMenuItemId { get; set; }
    public MenuItemType ItemType { get; set; }
    public string ItemTypeDisplay { get; set; } = string.Empty;
    public int? ScreenId { get; set; }
    public string? ScreenName { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public string? BadgeText { get; set; }
    public string? BadgeColor { get; set; }
    public List<MenuItemDto> Children { get; set; } = new();
    public bool HasPermission { get; set; } = true; // Kullanıcı izni var mı?
}

public class CreateMenuItemDto
{
    [Required(ErrorMessage = "Menu ID is required")]
    public int MenuId { get; set; }

    public int? ParentMenuItemId { get; set; }

    [Required(ErrorMessage = "Item type is required")]
    public MenuItemType ItemType { get; set; }

    public int? ScreenId { get; set; }

    [Required(ErrorMessage = "Display name is required")]
    [StringLength(200, ErrorMessage = "Display name cannot exceed 200 characters")]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    [StringLength(500, ErrorMessage = "Route cannot exceed 500 characters")]
    public string? Route { get; set; }

    [Range(0, 9999, ErrorMessage = "Order must be between 0 and 9999")]
    public int Order { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    [StringLength(50, ErrorMessage = "Badge text cannot exceed 50 characters")]
    public string? BadgeText { get; set; }

    [StringLength(50, ErrorMessage = "Badge color cannot exceed 50 characters")]
    public string? BadgeColor { get; set; }
}

public class UpdateMenuItemDto
{
    public int? ParentMenuItemId { get; set; }

    [Required(ErrorMessage = "Item type is required")]
    public MenuItemType ItemType { get; set; }

    public int? ScreenId { get; set; }

    [Required(ErrorMessage = "Display name is required")]
    [StringLength(200, ErrorMessage = "Display name cannot exceed 200 characters")]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    [StringLength(500, ErrorMessage = "Route cannot exceed 500 characters")]
    public string? Route { get; set; }

    [Range(0, 9999, ErrorMessage = "Order must be between 0 and 9999")]
    public int Order { get; set; }

    public bool IsActive { get; set; }

    [StringLength(50, ErrorMessage = "Badge text cannot exceed 50 characters")]
    public string? BadgeText { get; set; }

    [StringLength(50, ErrorMessage = "Badge color cannot exceed 50 characters")]
    public string? BadgeColor { get; set; }
}

public class UserMenusDto
{
    public List<UserMenuDto> SidebarMenus { get; set; } = new();
    public List<UserMenuDto> HeaderMenus { get; set; } = new();
}

public class UserMenuDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public MenuLocation Location { get; set; }
    public int Order { get; set; }
    public List<MenuItemDto> Items { get; set; } = new();
}
