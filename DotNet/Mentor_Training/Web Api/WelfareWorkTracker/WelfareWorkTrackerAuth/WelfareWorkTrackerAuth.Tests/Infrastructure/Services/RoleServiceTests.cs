namespace WelfareWorkTrackerAuth.Tests.Infrastructure.Services;
public class RoleServiceTests
{
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly RoleService _roleService;

    public RoleServiceTests()
    {
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockMapper = new Mock<IMapper>();
        _roleService = new RoleService(_mockRoleRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task AddRoleAsync_WithValidRoleVm_ReturnsMappedRoleDto()
    {
        // Arrange
        var roleVm = CreateRoleVm("Admin");
        var roleEntity = CreateRole(1, roleVm.RoleName);
        var expectedDto = CreateRoleDto(roleEntity.RoleId, roleEntity.RoleName);

        _mockRoleRepository
            .Setup(r => r.AddRoleAsync(It.Is<Role>(x => x.RoleName == roleVm.RoleName)))
            .ReturnsAsync(roleEntity);

        _mockMapper
            .Setup(m => m.Map<RoleDto>(roleEntity))
            .Returns(expectedDto);

        // Act
        var result = await _roleService.AddRoleAsync(roleVm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.RoleId, result.RoleId);
        Assert.Equal(expectedDto.RoleName, result.RoleName);

        _mockRoleRepository.Verify(r => r.AddRoleAsync(It.IsAny<Role>()), Times.Once);
        _mockMapper.Verify(m => m.Map<RoleDto>(roleEntity), Times.Once);
    }

    [Fact]
    public async Task GetRolesAsync_WithExistingRoles_ReturnsMappedRoleDtos()
    {
        // Arrange
        var roles = new List<Role>
        {
            CreateRole(1, "Admin"),
            CreateRole(2, "Manager")
        };

        var expectedDtos = new List<RoleDto>
        {
            CreateRoleDto(1, "Admin"),
            CreateRoleDto(2, "Manager")
        };

        _mockRoleRepository
            .Setup(r => r.GetRolesAsync())
            .ReturnsAsync(roles);

        _mockMapper
            .Setup(m => m.Map<RoleDto>(roles[0]))
            .Returns(expectedDtos[0]);

        _mockMapper
            .Setup(m => m.Map<RoleDto>(roles[1]))
            .Returns(expectedDtos[1]);

        // Act
        var result = await _roleService.GetRolesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDtos.Count, result.Count);
        Assert.Equal(expectedDtos[0].RoleId, result[0].RoleId);
        Assert.Equal(expectedDtos[0].RoleName, result[0].RoleName);
        Assert.Equal(expectedDtos[1].RoleId, result[1].RoleId);
        Assert.Equal(expectedDtos[1].RoleName, result[1].RoleName);

        _mockRoleRepository.Verify(r => r.GetRolesAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<RoleDto>(It.IsAny<Role>()), Times.Exactly(roles.Count));
    }

    [Fact]
    public async Task GetRolesAsync_WithNoRoles_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        _mockRoleRepository
            .Setup(r => r.GetRolesAsync())
            .ReturnsAsync(new List<Role>());

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _roleService.GetRolesAsync());

        // Assert
        Assert.Equal("Roles not found.", exception.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);

        _mockMapper.Verify(m => m.Map<RoleDto>(It.IsAny<Role>()), Times.Never);
    }

    private static RoleVm CreateRoleVm(string roleName)
    {
        return new RoleVm
        {
            RoleName = roleName
        };
    }

    private static Role CreateRole(int roleId, string roleName)
    {
        return new Role
        {
            RoleId = roleId,
            RoleName = roleName
        };
    }

    private static RoleDto CreateRoleDto(int roleId, string roleName)
    {
        return new RoleDto
        {
            RoleId = roleId,
            RoleName = roleName
        };
    }
}
