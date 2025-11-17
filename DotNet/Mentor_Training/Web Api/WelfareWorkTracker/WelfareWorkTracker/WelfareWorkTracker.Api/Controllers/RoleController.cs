namespace WelfareWorkTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    /// <summary>
    /// Retrieves all roles available in the system.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of all created roles.  
    /// If no roles exist, the service will throw an exception handled globally.
    /// </remarks>
    /// <returns>
    /// Returns an <see cref="IActionResult"/> containing the list of roles.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleService.GetRolesAsync();
        return Ok(roles);
    }

    /// <summary>
    /// Creates a new role in the system.
    /// </summary>
    /// <param name="role">The role information to be created.</param>
    /// <remarks>
    /// The request body must contain a valid <see cref="RoleVm"/>.
    /// </remarks>
    /// <returns>
    /// Returns a success message and the created role details.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> AddRole(RoleVm role)
    {
        var createdRole = await _roleService.AddRoleAsync(role);
        return Ok(new { message = "role created successfully", createdRole });
    }
}
