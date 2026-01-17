using JwtBackendApi.Models.DTOs;

namespace JwtBackendApi.Services;

public interface IScreenService
{
    // Screen operations
    Task<List<ScreenDto>> GetAllScreensAsync();
    Task<ScreenDto?> GetScreenByIdAsync(int id);
    Task<ScreenDto?> GetScreenByNameAsync(string name);
    Task<ScreenDto> CreateScreenAsync(CreateScreenDto dto);
    Task<ScreenDto?> UpdateScreenAsync(int id, UpdateScreenDto dto);
    Task<bool> DeleteScreenAsync(int id);

    // Screen Action operations
    Task<List<ScreenActionDto>> GetScreenActionsAsync(int screenId);
    Task<ScreenActionDto?> GetScreenActionByIdAsync(int id);
    Task<ScreenActionDto> CreateScreenActionAsync(CreateScreenActionDto dto);
    Task<ScreenActionDto?> UpdateScreenActionAsync(int id, UpdateScreenActionDto dto);
    Task<bool> DeleteScreenActionAsync(int id);
}
