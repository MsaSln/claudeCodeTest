using JwtBackendApi.Models;
using JwtBackendApi.Models.DTOs;

namespace JwtBackendApi.Services;

public class ScreenService : IScreenService
{
    private readonly List<Screen> _screens;
    private readonly List<ScreenAction> _screenActions;

    public ScreenService()
    {
        _screens = new List<Screen>();
        _screenActions = new List<ScreenAction>();

        // Seed default screens and actions
        SeedDefaultData();
    }

    private void SeedDefaultData()
    {
        // Seed Screens
        _screens.AddRange(new[]
        {
            new Screen
            {
                Id = 1,
                Name = "Dashboard",
                DisplayName = "Ana Sayfa",
                Description = "Dashboard ekranı",
                Route = "/dashboard",
                Icon = "dashboard",
                Order = 1,
                IsActive = true
            },
            new Screen
            {
                Id = 2,
                Name = "Users",
                DisplayName = "Kullanıcılar",
                Description = "Kullanıcı yönetimi ekranı",
                Route = "/users",
                Icon = "people",
                Order = 2,
                IsActive = true
            },
            new Screen
            {
                Id = 3,
                Name = "Settings",
                DisplayName = "Ayarlar",
                Description = "Sistem ayarları",
                Route = "/settings",
                Icon = "settings",
                Order = 3,
                IsActive = true
            }
        });

        // Seed Screen Actions
        _screenActions.AddRange(new[]
        {
            // Dashboard Actions
            new ScreenAction { Id = 1, ScreenId = 1, ActionName = "view", DisplayName = "Görüntüle", IsActive = true },
            new ScreenAction { Id = 2, ScreenId = 1, ActionName = "export", DisplayName = "Dışa Aktar", IsActive = true },

            // Users Actions
            new ScreenAction { Id = 3, ScreenId = 2, ActionName = "view", DisplayName = "Görüntüle", IsActive = true },
            new ScreenAction { Id = 4, ScreenId = 2, ActionName = "create", DisplayName = "Oluştur", IsActive = true },
            new ScreenAction { Id = 5, ScreenId = 2, ActionName = "edit", DisplayName = "Düzenle", IsActive = true },
            new ScreenAction { Id = 6, ScreenId = 2, ActionName = "delete", DisplayName = "Sil", IsActive = true },
            new ScreenAction { Id = 7, ScreenId = 2, ActionName = "export", DisplayName = "Dışa Aktar", IsActive = true },

            // Settings Actions
            new ScreenAction { Id = 8, ScreenId = 3, ActionName = "view", DisplayName = "Görüntüle", IsActive = true },
            new ScreenAction { Id = 9, ScreenId = 3, ActionName = "edit", DisplayName = "Düzenle", IsActive = true }
        });
    }

    // Screen Operations
    public async Task<List<ScreenDto>> GetAllScreensAsync()
    {
        var screenDtos = _screens.Select(s => MapToDto(s, true)).ToList();
        return await Task.FromResult(screenDtos);
    }

    public async Task<ScreenDto?> GetScreenByIdAsync(int id)
    {
        var screen = _screens.FirstOrDefault(s => s.Id == id);
        return await Task.FromResult(screen != null ? MapToDto(screen, true) : null);
    }

    public async Task<ScreenDto?> GetScreenByNameAsync(string name)
    {
        var screen = _screens.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return await Task.FromResult(screen != null ? MapToDto(screen, true) : null);
    }

    public async Task<ScreenDto> CreateScreenAsync(CreateScreenDto dto)
    {
        var screen = new Screen
        {
            Id = _screens.Count > 0 ? _screens.Max(s => s.Id) + 1 : 1,
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Description = dto.Description,
            Route = dto.Route,
            Icon = dto.Icon,
            ParentScreenId = dto.ParentScreenId,
            Order = dto.Order,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _screens.Add(screen);
        return await Task.FromResult(MapToDto(screen, true));
    }

    public async Task<ScreenDto?> UpdateScreenAsync(int id, UpdateScreenDto dto)
    {
        var screen = _screens.FirstOrDefault(s => s.Id == id);
        if (screen == null)
            return null;

        screen.Name = dto.Name;
        screen.DisplayName = dto.DisplayName;
        screen.Description = dto.Description;
        screen.Route = dto.Route;
        screen.Icon = dto.Icon;
        screen.ParentScreenId = dto.ParentScreenId;
        screen.Order = dto.Order;
        screen.IsActive = dto.IsActive;
        screen.UpdatedAt = DateTime.UtcNow;

        return await Task.FromResult(MapToDto(screen, true));
    }

    public async Task<bool> DeleteScreenAsync(int id)
    {
        var screen = _screens.FirstOrDefault(s => s.Id == id);
        if (screen == null)
            return false;

        _screens.Remove(screen);
        _screenActions.RemoveAll(a => a.ScreenId == id);

        return await Task.FromResult(true);
    }

    // Screen Action Operations
    public async Task<List<ScreenActionDto>> GetScreenActionsAsync(int screenId)
    {
        var actions = _screenActions
            .Where(a => a.ScreenId == screenId)
            .Select(MapActionToDto)
            .ToList();

        return await Task.FromResult(actions);
    }

    public async Task<ScreenActionDto?> GetScreenActionByIdAsync(int id)
    {
        var action = _screenActions.FirstOrDefault(a => a.Id == id);
        return await Task.FromResult(action != null ? MapActionToDto(action) : null);
    }

    public async Task<ScreenActionDto> CreateScreenActionAsync(CreateScreenActionDto dto)
    {
        var action = new ScreenAction
        {
            Id = _screenActions.Count > 0 ? _screenActions.Max(a => a.Id) + 1 : 1,
            ScreenId = dto.ScreenId,
            ActionName = dto.ActionName,
            DisplayName = dto.DisplayName,
            Description = dto.Description,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _screenActions.Add(action);
        return await Task.FromResult(MapActionToDto(action));
    }

    public async Task<ScreenActionDto?> UpdateScreenActionAsync(int id, UpdateScreenActionDto dto)
    {
        var action = _screenActions.FirstOrDefault(a => a.Id == id);
        if (action == null)
            return null;

        action.ActionName = dto.ActionName;
        action.DisplayName = dto.DisplayName;
        action.Description = dto.Description;
        action.IsActive = dto.IsActive;

        return await Task.FromResult(MapActionToDto(action));
    }

    public async Task<bool> DeleteScreenActionAsync(int id)
    {
        var action = _screenActions.FirstOrDefault(a => a.Id == id);
        if (action == null)
            return false;

        _screenActions.Remove(action);
        return await Task.FromResult(true);
    }

    // Mapping methods
    private ScreenDto MapToDto(Screen screen, bool includeActions = false)
    {
        var dto = new ScreenDto
        {
            Id = screen.Id,
            Name = screen.Name,
            DisplayName = screen.DisplayName,
            Description = screen.Description,
            Route = screen.Route,
            Icon = screen.Icon,
            ParentScreenId = screen.ParentScreenId,
            Order = screen.Order,
            IsActive = screen.IsActive
        };

        if (includeActions)
        {
            dto.Actions = _screenActions
                .Where(a => a.ScreenId == screen.Id)
                .Select(MapActionToDto)
                .ToList();
        }

        return dto;
    }

    private ScreenActionDto MapActionToDto(ScreenAction action)
    {
        return new ScreenActionDto
        {
            Id = action.Id,
            ScreenId = action.ScreenId,
            ActionName = action.ActionName,
            DisplayName = action.DisplayName,
            Description = action.Description,
            IsActive = action.IsActive
        };
    }
}
