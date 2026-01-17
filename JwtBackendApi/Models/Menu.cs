namespace JwtBackendApi.Models;

/// <summary>
/// Menü tanımı - Sidebar veya Header menüleri
/// </summary>
public class Menu
{
    public int Id { get; set; }

    /// <summary>
    /// Menü adı (örn: "Main Menu", "Admin Menu")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Görünen başlık
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Açıklama
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Menünün konumu (Sidebar/Header)
    /// </summary>
    public MenuLocation Location { get; set; }

    /// <summary>
    /// Sıralama
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
}
