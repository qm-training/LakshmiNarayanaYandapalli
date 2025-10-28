using AutoMapper;
using System.Net;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Contracts.Service;
using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Exceptions;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Infrastructure.Service
{
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
            var roles = await _roleRepository.GetAllRolesAsync();
            if (roles == null || roles.Count == 0)
                throw new WelfareWorkTrackerException("No roles found", (int)HttpStatusCode.NotFound);

            var roleDtos = new List<RoleDto>();
            foreach(var role in roles)
            {
                var roleDto = _mapper.Map<RoleDto>(role);
                roleDtos.Add(roleDto);
            }
            return roleDtos;
        }
    }
}
