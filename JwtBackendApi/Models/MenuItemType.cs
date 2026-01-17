namespace JwtBackendApi.Models;

/// <summary>
/// Menü elemanının tipini belirtir
/// </summary>
public enum MenuItemType
{
    /// <summary>
    /// Alt menü grubu (başlık/kategori)
    /// </summary>
    Group = 1,

    /// <summary>
    /// Direkt ekran bağlantısı
    /// </summary>
    Screen = 2
}
