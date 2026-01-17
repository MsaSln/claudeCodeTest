using System.ComponentModel.DataAnnotations;

namespace JwtBackendApi.Models.DTOs;

public class UserGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateUserGroupDto
{
    [Required(ErrorMessage = "Group name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Group name must be between 3 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

public class UpdateUserGroupDto
{
    [Required(ErrorMessage = "Group name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Group name must be between 3 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
