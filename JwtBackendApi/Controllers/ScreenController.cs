using JwtBackendApi.Models.DTOs;
using JwtBackendApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtBackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScreenController : ControllerBase
{
    private readonly IScreenService _screenService;
    private readonly ILogger<ScreenController> _logger;

    public ScreenController(
        IScreenService screenService,
        ILogger<ScreenController> logger)
    {
        _screenService = screenService;
        _logger = logger;
    }

    #region Screen Management

    /// <summary>
    /// Get all screens with their actions
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ScreenDto>>> GetAllScreens()
    {
        var screens = await _screenService.GetAllScreensAsync();
        return Ok(screens);
    }

    /// <summary>
    /// Get screen by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ScreenDto>> GetScreenById(int id)
    {
        var screen = await _screenService.GetScreenByIdAsync(id);
        if (screen == null)
            return NotFound(new { message = "Screen not found" });

        return Ok(screen);
    }

    /// <summary>
    /// Get screen by name
    /// </summary>
    [HttpGet("by-name/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ScreenDto>> GetScreenByName(string name)
    {
        var screen = await _screenService.GetScreenByNameAsync(name);
        if (screen == null)
            return NotFound(new { message = "Screen not found" });

        return Ok(screen);
    }

    /// <summary>
    /// Create a new screen
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ScreenDto>> CreateScreen([FromBody] CreateScreenDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var screen = await _screenService.CreateScreenAsync(dto);

        _logger.LogInformation("Screen created: {ScreenName}", screen.Name);

        return CreatedAtAction(
            nameof(GetScreenById),
            new { id = screen.Id },
            screen);
    }

    /// <summary>
    /// Update an existing screen
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ScreenDto>> UpdateScreen(int id, [FromBody] UpdateScreenDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var screen = await _screenService.UpdateScreenAsync(id, dto);
        if (screen == null)
            return NotFound(new { message = "Screen not found" });

        _logger.LogInformation("Screen updated: {ScreenId}", id);

        return Ok(screen);
    }

    /// <summary>
    /// Delete a screen
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteScreen(int id)
    {
        var result = await _screenService.DeleteScreenAsync(id);
        if (!result)
            return NotFound(new { message = "Screen not found" });

        _logger.LogInformation("Screen deleted: {ScreenId}", id);

        return Ok(new { message = "Screen deleted successfully" });
    }

    #endregion

    #region Screen Action Management

    /// <summary>
    /// Get all actions for a specific screen
    /// </summary>
    [HttpGet("{screenId}/actions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ScreenActionDto>>> GetScreenActions(int screenId)
    {
        var actions = await _screenService.GetScreenActionsAsync(screenId);
        return Ok(actions);
    }

    /// <summary>
    /// Get screen action by ID
    /// </summary>
    [HttpGet("actions/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ScreenActionDto>> GetScreenActionById(int id)
    {
        var action = await _screenService.GetScreenActionByIdAsync(id);
        if (action == null)
            return NotFound(new { message = "Screen action not found" });

        return Ok(action);
    }

    /// <summary>
    /// Create a new screen action
    /// </summary>
    [HttpPost("actions")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ScreenActionDto>> CreateScreenAction([FromBody] CreateScreenActionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verify screen exists
        var screen = await _screenService.GetScreenByIdAsync(dto.ScreenId);
        if (screen == null)
            return BadRequest(new { message = "Screen not found" });

        var action = await _screenService.CreateScreenActionAsync(dto);

        _logger.LogInformation("Screen action created: {ActionName} for screen {ScreenId}", action.ActionName, action.ScreenId);

        return CreatedAtAction(
            nameof(GetScreenActionById),
            new { id = action.Id },
            action);
    }

    /// <summary>
    /// Update an existing screen action
    /// </summary>
    [HttpPut("actions/{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ScreenActionDto>> UpdateScreenAction(int id, [FromBody] UpdateScreenActionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var action = await _screenService.UpdateScreenActionAsync(id, dto);
        if (action == null)
            return NotFound(new { message = "Screen action not found" });

        _logger.LogInformation("Screen action updated: {ActionId}", id);

        return Ok(action);
    }

    /// <summary>
    /// Delete a screen action
    /// </summary>
    [HttpDelete("actions/{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteScreenAction(int id)
    {
        var result = await _screenService.DeleteScreenActionAsync(id);
        if (!result)
            return NotFound(new { message = "Screen action not found" });

        _logger.LogInformation("Screen action deleted: {ActionId}", id);

        return Ok(new { message = "Screen action deleted successfully" });
    }

    #endregion
}
