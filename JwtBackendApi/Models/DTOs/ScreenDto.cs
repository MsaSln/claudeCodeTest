using System.ComponentModel.DataAnnotations;

namespace JwtBackendApi.Models.DTOs;

public class ScreenDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Route { get; set; }
    public string? Icon { get; set; }
    public int? ParentScreenId { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public List<ScreenActionDto> Actions { get; set; } = new();
}

public class CreateScreenDto
{
    [Required(ErrorMessage = "Screen name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Screen name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Display name must be between 2 and 100 characters")]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Route cannot exceed 200 characters")]
    public string? Route { get; set; }

    [StringLength(50, ErrorMessage = "Icon cannot exceed 50 characters")]
    public string? Icon { get; set; }

    public int? ParentScreenId { get; set; }

    public int Order { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}

public class UpdateScreenDto
{
    [Required(ErrorMessage = "Screen name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Screen name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Display name must be between 2 and 100 characters")]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Route cannot exceed 200 characters")]
    public string? Route { get; set; }

    [StringLength(50, ErrorMessage = "Icon cannot exceed 50 characters")]
    public string? Icon { get; set; }

    public int? ParentScreenId { get; set; }

    public int Order { get; set; }

    public bool IsActive { get; set; }
}
