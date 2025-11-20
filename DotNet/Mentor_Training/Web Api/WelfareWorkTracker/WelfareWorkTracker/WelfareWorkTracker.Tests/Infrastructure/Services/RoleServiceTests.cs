namespace WelfareWorkTracker.Tests.Infrastructure.Services;
public class RoleServiceTests
{
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly RoleService _service;

    public RoleServiceTests()
    {
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new RoleService(_mockRoleRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task AddRoleAsync_WithValidInput_ReturnsMappedDto()
    {
        // Arrange
        var vm = new RoleVm { RoleName = "Admin" };
        var saved = new Role { RoleId = 10, RoleName = "Admin" };
        var dto = new RoleDto { RoleId = 10, RoleName = "Admin" };

        _mockRoleRepository
            .Setup(r => r.AddRoleAsync(It.Is<Role>(x => x.RoleName == vm.RoleName)))
            .ReturnsAsync(saved);

        _mockMapper
            .Setup(m => m.Map<RoleDto>(saved))
            .Returns(dto);

        // Act
        var result = await _service.AddRoleAsync(vm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.RoleId);
        Assert.Equal("Admin", result.RoleName);

        _mockRoleRepository.Verify(r => r.AddRoleAsync(It.Is<Role>(x => x.RoleName == "Admin")), Times.Once);
        _mockMapper.Verify(m => m.Map<RoleDto>(saved), Times.Once);
    }

    [Fact]
    public async Task GetRolesAsync_WithExistingRoles_ReturnsMappedDtos()
    {
        // Arrange
        var roles = new List<Role>
            {
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "User" }
            };

        _mockRoleRepository
            .Setup(r => r.GetAllRolesAsync())
            .ReturnsAsync(roles);

        _mockMapper
            .Setup(m => m.Map<RoleDto>(It.IsAny<Role>()))
            .Returns<Role>(r => new RoleDto { RoleId = r.RoleId, RoleName = r.RoleName });

        // Act
        var result = await _service.GetRolesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.RoleId == 1 && d.RoleName == "Admin");
        Assert.Contains(result, d => d.RoleId == 2 && d.RoleName == "User");

        _mockRoleRepository.Verify(r => r.GetAllRolesAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<RoleDto>(It.IsAny<Role>()), Times.Exactly(roles.Count));
    }

    [Fact]
    public async Task GetRolesAsync_WithNoRoles_ThrowsNotFound()
    {
        // Arrange
        _mockRoleRepository
            .Setup(r => r.GetAllRolesAsync())
            .ReturnsAsync(new List<Role>());

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _service.GetRolesAsync());

        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
        Assert.Equal("No roles found", ex.Message);

        _mockRoleRepository.Verify(r => r.GetAllRolesAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<RoleDto>(It.IsAny<Role>()), Times.Never);
    }
}
