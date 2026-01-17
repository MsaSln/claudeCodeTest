namespace JwtBackendApi.Models;

/// <summary>
/// Ekran izin modeli - UserGroup'un hangi Screen'de hangi Action'lara erişebildiği
/// </summary>
public class ScreenPermission
{
    public int Id { get; set; }
    public int UserGroupId { get; set; }
    public int ScreenId { get; set; }
    public int ScreenActionId { get; set; }
    public bool IsGranted { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
}
