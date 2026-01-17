namespace JwtBackendApi.Models;

/// <summary>
/// Menü elemanı - Grup veya Ekran olabilir
/// </summary>
public class MenuItem
{
    public int Id { get; set; }

    /// <summary>
    /// Bağlı olduğu menü
    /// </summary>
    public int MenuId { get; set; }

    /// <summary>
    /// Üst menü elemanı (hiyerarşik yapı için)
    /// Group tipindeki item'lar altında başka item'lar olabilir
    /// </summary>
    public int? ParentMenuItemId { get; set; }

    /// <summary>
    /// Menü elemanı tipi (Group/Screen)
    /// </summary>
    public MenuItemType ItemType { get; set; }

    /// <summary>
    /// Eğer ItemType = Screen ise, hangi ekrana bağlı
    /// </summary>
    public int? ScreenId { get; set; }

    /// <summary>
    /// Görünen başlık
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// İkon
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Route/URL (ItemType = Screen ise gerekli)
    /// </summary>
    public string? Route { get; set; }

    /// <summary>
    /// Sıralama
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Badge text (örn: "New", "Beta", sayı gösterimi için)
    /// </summary>
    public string? BadgeText { get; set; }

    /// <summary>
    /// Badge rengi
    /// </summary>
    public string? BadgeColor { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
}
