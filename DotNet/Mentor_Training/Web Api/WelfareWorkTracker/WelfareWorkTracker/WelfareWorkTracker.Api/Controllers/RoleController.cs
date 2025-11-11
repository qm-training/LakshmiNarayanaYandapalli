namespace WelfareWorkTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleService.GetRolesAsync();
        return Ok(roles);
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(RoleVm role)
    {
        var createdRole = await _roleService.AddRoleAsync(role);
        return Ok(new { message = "role created successfully", createdRole });
    }
}
