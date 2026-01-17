namespace JwtBackendApi.Models;

/// <summary>
/// Ekran (sayfa/modül) modeli
/// </summary>
public class Screen
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Route { get; set; }
    public string? Icon { get; set; }
    public int? ParentScreenId { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
