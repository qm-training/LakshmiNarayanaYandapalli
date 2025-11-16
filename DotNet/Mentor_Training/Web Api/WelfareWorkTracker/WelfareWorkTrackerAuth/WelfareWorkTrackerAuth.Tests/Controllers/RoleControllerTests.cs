using WelfareWorkTrackerAuth.Core.Dtos;

namespace WelfareWorkTrackerAuth.Tests.Controllers;
public class RoleControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRoleService> _mockRoleService;
    private readonly RoleController _roleController;

    public RoleControllerTests()
    {
        _fixture = new Fixture();
        _mockRoleService = new Mock<IRoleService>();
        _roleController = new RoleController(_mockRoleService.Object);
    }

    [Fact]
    public async Task GetRolesAsync_WithExistingRoles_ReturnsOkObjectResultWithRoles()
    {
        // Arrange
        var expectedRoles = _fixture.CreateMany<RoleDto>(3).ToList();
        _mockRoleService
            .Setup(service => service.GetRolesAsync())
            .ReturnsAsync(expectedRoles);

        // Act
        var result = await _roleController.GetRolesAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RoleDto>>(okResult.Value);
        Assert.Equal(expectedRoles, value);
    }

    [Fact]
    public async Task GetRolesAsync_WithServiceThrowsException_ThrowsException()
    {
        // Arrange
        var expectedException = new Exception("Failed to get roles");
        _mockRoleService
            .Setup(service => service.GetRolesAsync())
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _roleController.GetRolesAsync());
        Assert.Equal(expectedException.Message, exception.Message);
    }

    [Fact]
    public async Task AddRoleAsync_WithValidRole_ReturnsOkObjectResultWithCreatedRole()
    {
        // Arrange
        var roleVm = _fixture.Create<RoleVm>();
        var expectedResult = _fixture.Create<RoleDto>();

        _mockRoleService
            .Setup(service => service.AddRoleAsync(roleVm))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _roleController.AddRoleAsync(roleVm);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResult, okResult.Value);
    }


    [Fact]
    public async Task AddRoleAsync_WithServiceThrowsException_ThrowsException()
    {
        // Arrange
        var roleVm = _fixture.Create<RoleVm>();
        var expectedException = new Exception("Failed to add role");

        _mockRoleService
            .Setup(service => service.AddRoleAsync(roleVm))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _roleController.AddRoleAsync(roleVm));
        Assert.Equal(expectedException.Message, exception.Message);
    }
}
