using System.ComponentModel.DataAnnotations;

namespace JwtBackendApi.Models.DTOs;

public class ScreenActionDto
{
    public int Id { get; set; }
    public int ScreenId { get; set; }
    public string ActionName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class CreateScreenActionDto
{
    [Required(ErrorMessage = "Screen ID is required")]
    public int ScreenId { get; set; }

    [Required(ErrorMessage = "Action name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Action name must be between 2 and 100 characters")]
    public string ActionName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Display name must be between 2 and 100 characters")]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateScreenActionDto
{
    [Required(ErrorMessage = "Action name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Action name must be between 2 and 100 characters")]
    public string ActionName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Display name must be between 2 and 100 characters")]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
