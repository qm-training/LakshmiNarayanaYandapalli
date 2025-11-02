namespace WelfareWorkTrackerAuth.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;
    [HttpGet]
    public async Task<IActionResult> GetRolesAsync()
    {
        var result = await _roleService.GetRolesAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddRoleAsync(RoleVm roleVm)
    {
        var result = await _roleService.AddRoleAsync(roleVm);
        return Ok(result);
    }
}
