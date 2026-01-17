namespace JwtBackendApi.Models;

/// <summary>
/// Ekran aksiyonu modeli (view, create, edit, delete, export, vb.)
/// </summary>
public class ScreenAction
{
    public int Id { get; set; }
    public int ScreenId { get; set; }
    public string ActionName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
