using JwtBackendApi.Models.DTOs;
using JwtBackendApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JwtBackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;
    private readonly ILogger<MenuController> _logger;

    public MenuController(IMenuService menuService, ILogger<MenuController> logger)
    {
        _menuService = menuService;
        _logger = logger;
    }

    #region Menu Management (Admin Only)

    /// <summary>
    /// Get all menus
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<MenuDto>>> GetAllMenus()
    {
        var menus = await _menuService.GetAllMenusAsync();
        return Ok(menus);
    }

    /// <summary>
    /// Get menu by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MenuDto>> GetMenuById(int id)
    {
        var menu = await _menuService.GetMenuByIdAsync(id);
        if (menu == null)
            return NotFound(new { message = "Menu not found" });

        return Ok(menu);
    }

    /// <summary>
    /// Create a new menu
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MenuDto>> CreateMenu([FromBody] CreateMenuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        var menu = await _menuService.CreateMenuAsync(dto, userId);

        _logger.LogInformation("Menu created: {MenuName} by user {UserId}", menu.Name, userId);

        return CreatedAtAction(nameof(GetMenuById), new { id = menu.Id }, menu);
    }

    /// <summary>
    /// Update an existing menu
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MenuDto>> UpdateMenu(int id, [FromBody] UpdateMenuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        var menu = await _menuService.UpdateMenuAsync(id, dto, userId);

        if (menu == null)
            return NotFound(new { message = "Menu not found" });

        _logger.LogInformation("Menu updated: {MenuId} by user {UserId}", id, userId);

        return Ok(menu);
    }

    /// <summary>
    /// Delete a menu
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteMenu(int id)
    {
        var result = await _menuService.DeleteMenuAsync(id);
        if (!result)
            return NotFound(new { message = "Menu not found" });

        _logger.LogInformation("Menu deleted: {MenuId}", id);

        return Ok(new { message = "Menu deleted successfully" });
    }

    #endregion

    #region MenuItem Management (Admin Only)

    /// <summary>
    /// Get menu item by ID
    /// </summary>
    [HttpGet("items/{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MenuItemDto>> GetMenuItemById(int id)
    {
        var menuItem = await _menuService.GetMenuItemByIdAsync(id);
        if (menuItem == null)
            return NotFound(new { message = "Menu item not found" });

        return Ok(menuItem);
    }

    /// <summary>
    /// Get all menu items for a specific menu
    /// </summary>
    [HttpGet("{menuId}/items")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<MenuItemDto>>> GetMenuItems(int menuId)
    {
        var menuItems = await _menuService.GetMenuItemsByMenuIdAsync(menuId);
        return Ok(menuItems);
    }

    /// <summary>
    /// Create a new menu item
    /// </summary>
    [HttpPost("items")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MenuItemDto>> CreateMenuItem([FromBody] CreateMenuItemDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = GetCurrentUserId();
            var menuItem = await _menuService.CreateMenuItemAsync(dto, userId);

            _logger.LogInformation(
                "Menu item created: {DisplayName} in menu {MenuId} by user {UserId}",
                menuItem.DisplayName, dto.MenuId, userId);

            return CreatedAtAction(
                nameof(GetMenuItemById),
                new { id = menuItem.Id },
                menuItem);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing menu item
    /// </summary>
    [HttpPut("items/{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MenuItemDto>> UpdateMenuItem(int id, [FromBody] UpdateMenuItemDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = GetCurrentUserId();
            var menuItem = await _menuService.UpdateMenuItemAsync(id, dto, userId);

            if (menuItem == null)
                return NotFound(new { message = "Menu item not found" });

            _logger.LogInformation("Menu item updated: {ItemId} by user {UserId}", id, userId);

            return Ok(menuItem);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a menu item (and its children)
    /// </summary>
    [HttpDelete("items/{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteMenuItem(int id)
    {
        var result = await _menuService.DeleteMenuItemAsync(id);
        if (!result)
            return NotFound(new { message = "Menu item not found" });

        _logger.LogInformation("Menu item deleted: {ItemId}", id);

        return Ok(new { message = "Menu item deleted successfully" });
    }

    #endregion

    #region User Menu Access

    /// <summary>
    /// Get current user's menus with permission filtering
    /// Returns both sidebar and header menus
    /// </summary>
    [HttpGet("my-menus")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserMenusDto>> GetMyMenus()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(new { message = "User not authenticated" });

        var menus = await _menuService.GetUserMenusAsync(userId.Value);

        _logger.LogInformation("User {UserId} accessed their menus", userId.Value);

        return Ok(menus);
    }

    /// <summary>
    /// Get user's menus by user ID (for admin purposes)
    /// </summary>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserMenusDto>> GetUserMenus(int userId)
    {
        var menus = await _menuService.GetUserMenusAsync(userId);
        return Ok(menus);
    }

    /// <summary>
    /// Get specific menu items with permission filtering for a user
    /// </summary>
    [HttpGet("{menuId}/user/{userId}/items")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<MenuItemDto>>> GetUserMenuItems(int menuId, int userId)
    {
        var menuItems = await _menuService.GetMenuItemsWithPermissionsAsync(menuId, userId);
        return Ok(menuItems);
    }

    #endregion

    #region Helper Methods

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
            return userId;
        return null;
    }

    #endregion
}
