namespace WelfareWorkTrackerAuth.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    /// <summary>
    /// Retrieves all roles available in the authentication service.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of roles that are registered in the system.
    /// If no roles exist, the underlying service handles the response.
    /// </remarks>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the list of roles.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetRolesAsync()
    {
        var result = await _roleService.GetRolesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Adds a new role to the system.
    /// </summary>
    /// <param name="roleVm">The role information to be created.</param>
    /// <remarks>
    /// The request must contain a valid <see cref="RoleVm"/> with the required fields.
    /// </remarks>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the details of the created role.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> AddRoleAsync(RoleVm roleVm)
    {
        var result = await _roleService.AddRoleAsync(roleVm);
        return Ok(result);
    }
}