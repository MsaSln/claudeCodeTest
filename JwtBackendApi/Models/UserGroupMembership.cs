namespace JwtBackendApi.Models;

/// <summary>
/// Kullanıcı - Kullanıcı Grubu ilişki modeli (many-to-many)
/// </summary>
public class UserGroupMembership
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int UserGroupId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public int? AssignedBy { get; set; }
}
