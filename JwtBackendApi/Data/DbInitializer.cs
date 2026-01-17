using JwtBackendApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace JwtBackendApi.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // Check if data already exists
        if (context.UserGroups.Any())
        {
            return; // DB has been seeded
        }

        // Seed User Groups
        var userGroups = new UserGroup[]
        {
            new UserGroup
            {
                Name = "Administrators",
                Description = "Full system access",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new UserGroup
            {
                Name = "Managers",
                Description = "Management level access",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new UserGroup
            {
                Name = "Users",
                Description = "Standard user access",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };
        context.UserGroups.AddRange(userGroups);
        context.SaveChanges();

        // Seed Screens
        var screens = new Screen[]
        {
            new Screen
            {
                Name = "Dashboard",
                DisplayName = "Ana Sayfa",
                Description = "Dashboard ekranı",
                Route = "/dashboard",
                Icon = "dashboard",
                Order = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Screen
            {
                Name = "Users",
                DisplayName = "Kullanıcılar",
                Description = "Kullanıcı yönetimi ekranı",
                Route = "/users",
                Icon = "people",
                Order = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Screen
            {
                Name = "Settings",
                DisplayName = "Ayarlar",
                Description = "Sistem ayarları",
                Route = "/settings",
                Icon = "settings",
                Order = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };
        context.Screens.AddRange(screens);
        context.SaveChanges();

        // Seed Screen Actions
        var screenActions = new ScreenAction[]
        {
            // Dashboard Actions
            new ScreenAction { ScreenId = screens[0].Id, ActionName = "view", DisplayName = "Görüntüle", IsActive = true, CreatedAt = DateTime.UtcNow },
            new ScreenAction { ScreenId = screens[0].Id, ActionName = "export", DisplayName = "Dışa Aktar", IsActive = true, CreatedAt = DateTime.UtcNow },

            // Users Actions
            new ScreenAction { ScreenId = screens[1].Id, ActionName = "view", DisplayName = "Görüntüle", IsActive = true, CreatedAt = DateTime.UtcNow },
            new ScreenAction { ScreenId = screens[1].Id, ActionName = "create", DisplayName = "Oluştur", IsActive = true, CreatedAt = DateTime.UtcNow },
            new ScreenAction { ScreenId = screens[1].Id, ActionName = "edit", DisplayName = "Düzenle", IsActive = true, CreatedAt = DateTime.UtcNow },
            new ScreenAction { ScreenId = screens[1].Id, ActionName = "delete", DisplayName = "Sil", IsActive = true, CreatedAt = DateTime.UtcNow },
            new ScreenAction { ScreenId = screens[1].Id, ActionName = "export", DisplayName = "Dışa Aktar", IsActive = true, CreatedAt = DateTime.UtcNow },

            // Settings Actions
            new ScreenAction { ScreenId = screens[2].Id, ActionName = "view", DisplayName = "Görüntüle", IsActive = true, CreatedAt = DateTime.UtcNow },
            new ScreenAction { ScreenId = screens[2].Id, ActionName = "edit", DisplayName = "Düzenle", IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        context.ScreenActions.AddRange(screenActions);
        context.SaveChanges();

        // Seed Screen Permissions (Admin group gets all permissions)
        var permissions = new List<ScreenPermission>();

        // Admin group - all actions on all screens
        foreach (var action in screenActions)
        {
            permissions.Add(new ScreenPermission
            {
                UserGroupId = userGroups[0].Id, // Administrators
                ScreenId = action.ScreenId,
                ScreenActionId = action.Id,
                IsGranted = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Users group - view only on Dashboard and Users
        permissions.Add(new ScreenPermission
        {
            UserGroupId = userGroups[2].Id, // Users
            ScreenId = screens[0].Id, // Dashboard
            ScreenActionId = screenActions[0].Id, // view
            IsGranted = true,
            CreatedAt = DateTime.UtcNow
        });
        permissions.Add(new ScreenPermission
        {
            UserGroupId = userGroups[2].Id, // Users
            ScreenId = screens[1].Id, // Users
            ScreenActionId = screenActions[2].Id, // view
            IsGranted = true,
            CreatedAt = DateTime.UtcNow
        });

        context.ScreenPermissions.AddRange(permissions);
        context.SaveChanges();

        // Seed Menus
        var menus = new Menu[]
        {
            new Menu
            {
                Name = "MainSidebar",
                DisplayName = "Ana Menü",
                Description = "Sol taraf ana menü",
                Location = MenuLocation.Sidebar,
                Order = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Menu
            {
                Name = "TopHeader",
                DisplayName = "Üst Menü",
                Description = "Header menüsü",
                Location = MenuLocation.Header,
                Order = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };
        context.Menus.AddRange(menus);
        context.SaveChanges();

        // Seed Menu Items
        var menuItems = new List<MenuItem>();

        // Sidebar Menu Items
        var dashboardItem = new MenuItem
        {
            MenuId = menus[0].Id,
            ItemType = MenuItemType.Screen,
            ScreenId = screens[0].Id,
            DisplayName = "Ana Sayfa",
            Icon = "dashboard",
            Route = "/dashboard",
            Order = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        menuItems.Add(dashboardItem);

        var managementGroup = new MenuItem
        {
            MenuId = menus[0].Id,
            ItemType = MenuItemType.Group,
            DisplayName = "Yönetim",
            Icon = "settings",
            Order = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        menuItems.Add(managementGroup);

        context.MenuItems.AddRange(menuItems);
        context.SaveChanges();

        // Add child items to management group
        var usersItem = new MenuItem
        {
            MenuId = menus[0].Id,
            ParentMenuItemId = managementGroup.Id,
            ItemType = MenuItemType.Screen,
            ScreenId = screens[1].Id,
            DisplayName = "Kullanıcılar",
            Icon = "people",
            Route = "/users",
            Order = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var settingsItem = new MenuItem
        {
            MenuId = menus[0].Id,
            ParentMenuItemId = managementGroup.Id,
            ItemType = MenuItemType.Screen,
            ScreenId = screens[2].Id,
            DisplayName = "Ayarlar",
            Icon = "settings",
            Route = "/settings",
            Order = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.MenuItems.AddRange(new[] { usersItem, settingsItem });
        context.SaveChanges();

        // Header Menu Items
        var headerDashboard = new MenuItem
        {
            MenuId = menus[1].Id,
            ItemType = MenuItemType.Screen,
            ScreenId = screens[0].Id,
            DisplayName = "Dashboard",
            Icon = "dashboard",
            Route = "/dashboard",
            Order = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.MenuItems.Add(headerDashboard);
        context.SaveChanges();

        // Seed a default admin user
        var adminUser = new User
        {
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = HashPassword("Admin123!"),
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(adminUser);
        context.SaveChanges();

        // Assign admin to Administrators group
        var adminMembership = new UserGroupMembership
        {
            UserId = adminUser.Id,
            UserGroupId = userGroups[0].Id,
            AssignedAt = DateTime.UtcNow
        };
        context.UserGroupMemberships.Add(adminMembership);
        context.SaveChanges();
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
