using JwtBackendApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtBackendApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<UserGroupMembership> UserGroupMemberships { get; set; }
    public DbSet<Screen> Screens { get; set; }
    public DbSet<ScreenAction> ScreenActions { get; set; }
    public DbSet<ScreenPermission> ScreenPermissions { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // UserGroup configuration
        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.ToTable("user_groups");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // UserGroupMembership configuration
        modelBuilder.Entity<UserGroupMembership>(entity =>
        {
            entity.ToTable("user_group_memberships");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.UserGroupId }).IsUnique();
        });

        // Screen configuration
        modelBuilder.Entity<Screen>(entity =>
        {
            entity.ToTable("screens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Route).HasMaxLength(500);
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // ScreenAction configuration
        modelBuilder.Entity<ScreenAction>(entity =>
        {
            entity.ToTable("screen_actions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ActionName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => new { e.ScreenId, e.ActionName }).IsUnique();
        });

        // ScreenPermission configuration
        modelBuilder.Entity<ScreenPermission>(entity =>
        {
            entity.ToTable("screen_permissions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserGroupId, e.ScreenId, e.ScreenActionId }).IsUnique();
        });

        // Menu configuration
        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("menus");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // MenuItem configuration
        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.ToTable("menu_items");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.Route).HasMaxLength(500);
            entity.Property(e => e.BadgeText).HasMaxLength(50);
            entity.Property(e => e.BadgeColor).HasMaxLength(50);
        });
    }
}
