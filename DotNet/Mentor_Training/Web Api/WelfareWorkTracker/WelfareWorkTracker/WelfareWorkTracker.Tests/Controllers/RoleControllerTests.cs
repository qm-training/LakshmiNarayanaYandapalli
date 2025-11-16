namespace WelfareWorkTracker.Tests.Controllers;
public class RoleControllerTests
{
    private readonly Mock<IRoleService> _mockService;
    private readonly RoleController _controller;

    public RoleControllerTests()
    {
        _mockService = new Mock<IRoleService>();
        _controller = new RoleController(_mockService.Object);
    }

    [Fact]
    public async Task GetRoles_WithExistingData_ReturnsOkWithList()
    {
        // Arrange
        var list = new List<RoleDto>
            {
                new RoleDto { RoleId = 1, RoleName = "Admin" },
                new RoleDto { RoleId = 2, RoleName = "Citizen" }
            };
        _mockService.Setup(s => s.GetRolesAsync()).ReturnsAsync(list);

        // Act
        var result = await _controller.GetRoles();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal(list, ok.Value);
    }

    [Fact]
    public async Task GetRoles_WhenEmpty_ReturnsOkWithEmptyList()
    {
        // Arrange
        var empty = new List<RoleDto>();
        _mockService.Setup(s => s.GetRolesAsync()).ReturnsAsync(empty);

        // Act
        var result = await _controller.GetRoles();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal(empty, ok.Value);
    }

    [Fact]
    public async Task AddRole_WithValidInput_ReturnsOkWithPayload()
    {
        // Arrange
        var vm = CreateRoleVm("Manager");
        var dto = new RoleDto { RoleId = 10, RoleName = vm.RoleName };
        _mockService.Setup(s => s.AddRoleAsync(vm)).ReturnsAsync(dto);

        // Act
        var result = await _controller.AddRole(vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        var payload = ok.Value!;
        var message = payload.GetType().GetProperty("message")!.GetValue(payload, null);
        var createdRole = payload.GetType().GetProperty("createdRole")!.GetValue(payload, null);
        Assert.Equal("role created successfully", message);
        Assert.Equal(dto, createdRole);
    }

    [Fact]
    public async Task AddRole_WhenServiceReturnsNull_ReturnsOkWithNullCreatedRole()
    {
        // Arrange
        var vm = CreateRoleVm("Viewer");
        _mockService.Setup(s => s.AddRoleAsync(vm)).ReturnsAsync((RoleDto?)null);

        // Act
        var result = await _controller.AddRole(vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = ok.Value!;
        var message = payload.GetType().GetProperty("message")!.GetValue(payload, null);
        var createdRole = payload.GetType().GetProperty("createdRole")!.GetValue(payload, null);
        Assert.Equal("role created successfully", message);
        Assert.Null(createdRole);
    }
    private static RoleVm CreateRoleVm(string roleName)
        => new RoleVm { RoleName = roleName };
}
