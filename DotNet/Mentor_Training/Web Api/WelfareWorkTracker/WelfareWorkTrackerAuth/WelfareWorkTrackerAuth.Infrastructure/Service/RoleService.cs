namespace WelfareWorkTrackerAuth.Infrastructure.Service;
public class RoleService(IRoleRepository roleRepository,
                             IMapper mapper) : IRoleService
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IMapper _mapper = mapper;
    public async Task<RoleDto> AddRoleAsync(RoleVm roleVm)
    {
        var role = new Role
        {
            RoleName = roleVm.RoleName
        };

        var addedRole = await _roleRepository.AddRoleAsync(role);
        var roleDto = _mapper.Map<RoleDto>(addedRole);
        return roleDto;
    }

    public async Task<List<RoleDto>> GetRolesAsync()
    {
        var roles = await _roleRepository.GetRolesAsync();

        if (roles == null || roles.Count == 0)
        {
            throw new WelfareWorkTrackerException("Roles not found.", (int)HttpStatusCode.NotFound);
        }

        var rolesDto = new List<RoleDto>();
        foreach (var role in roles)
        {
            var roleDto = _mapper.Map<RoleDto>(role);
            rolesDto.Add(roleDto);
        }

        return rolesDto;
    }
}
