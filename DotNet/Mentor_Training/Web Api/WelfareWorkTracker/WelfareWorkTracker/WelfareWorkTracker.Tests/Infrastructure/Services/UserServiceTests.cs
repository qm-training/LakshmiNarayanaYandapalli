namespace WelfareWorkTracker.Tests.Infrastructure.Services;
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IRoleRepository> _mockRoleRepo;
    private readonly Mock<IConstituencyRepository> _mockConRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockRoleRepo = new Mock<IRoleRepository>();
        _mockConRepo = new Mock<IConstituencyRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new UserService(_mockUserRepo.Object, _mockRoleRepo.Object, _mockConRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetUserInfoByUserIdAsync_WithExistingUser_ReturnsDto()
    {
        // Arrange
        var user = CreateUser(10, 2, 100, "a@test.com", "Alex");
        var expected = new UserDto { UserId = 10, FullName = "Alex", Email = "a@test.com" };

        _mockUserRepo.Setup(r => r.GetUserByIdAsync(10)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(expected);

        // Act
        var result = await _service.GetUserInfoByUserIdAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.UserId);
        Assert.Equal("Alex", result.FullName);
        Assert.Equal("a@test.com", result.Email);

        _mockUserRepo.Verify(r => r.GetUserByIdAsync(10), Times.Once);
        _mockMapper.Verify(m => m.Map<UserDto>(user), Times.Once);
    }

    [Fact]
    public async Task GetUserInfoByUserIdAsync_UserNotFound_ThrowsNotFound()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetUserByIdAsync(99)).ReturnsAsync((User?)null);

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _service.GetUserInfoByUserIdAsync(99));

        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
        Assert.Equal("User not found.", ex.Message);
        _mockMapper.Verify(m => m.Map<UserDto>(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserInfoAsyncAsync_WithValidInput_UpdatesAndReturnsDto()
    {
        // Arrange
        var existing = CreateUser(11, 3, 200, "old@test.com", "Old Name");
        var vm = CreateUserVm("New Name", "new@test.com", 30, 5555555, "M", "123 St", "Leader", "Central");
        var roleId = 7;
        var constituencyId = 77;

        _mockUserRepo.Setup(r => r.GetUserByIdAsync(11)).ReturnsAsync(existing);
        _mockRoleRepo.Setup(r => r.GetRoleIdByRoleNameAsync(vm.RoleName)).ReturnsAsync(roleId);
        _mockConRepo.Setup(r => r.GetConstituencyIdByNameAsync(vm.ConstituencyName)).ReturnsAsync(constituencyId);

        User? captured = null;
        _mockUserRepo
            .Setup(r => r.UpdateUserAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u)
            .Callback<User>(u => captured = u);

        var expectedRoleId = (int)Enum.Parse(typeof(Roles), vm.RoleName, ignoreCase: true);
        var expected = new UserDto { UserId = 11, FullName = vm.FullName, Email = vm.Email, RoleId = expectedRoleId };
        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(expected);

        // Act
        var result = await _service.UpdateUserInfoAsyncAsync(11, vm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(11, result.UserId);
        Assert.Equal(vm.FullName, result.FullName);
        Assert.Equal(vm.Email, result.Email);

        Assert.NotNull(captured);
        Assert.Equal(vm.FullName, captured!.FullName);
        Assert.Equal(vm.Email, captured.Email);
        Assert.Equal(vm.Age, captured.Age);
        Assert.Equal(vm.MobileNumber, captured.MobileNumber);
        Assert.Equal(vm.Gender, captured.Gender);
        Assert.Equal(vm.Address, captured.Address);
        Assert.Equal(roleId, captured.RoleId);
        Assert.Equal(constituencyId, captured.ConstituencyId);

        _mockUserRepo.Verify(r => r.GetUserByIdAsync(11), Times.Once);
        _mockRoleRepo.Verify(r => r.GetRoleIdByRoleNameAsync(vm.RoleName), Times.Exactly(2));
        _mockConRepo.Verify(r => r.GetConstituencyIdByNameAsync(vm.ConstituencyName), Times.Exactly(2));
        _mockUserRepo.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Once);
        _mockMapper.Verify(m => m.Map<UserDto>(It.IsAny<User>()), Times.Once);
    }


    [Fact]
    public async Task UpdateUserInfoAsyncAsync_UserNotFound_ThrowsNotFound()
    {
        // Arrange
        var vm = CreateUserVm("New", "n@test.com", 20, 1, "F", "addr", "Role", "Cons");
        _mockUserRepo.Setup(r => r.GetUserByIdAsync(42)).ReturnsAsync((User?)null);

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _service.UpdateUserInfoAsyncAsync(42, vm));

        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
        Assert.Equal("User not found.", ex.Message);

        _mockRoleRepo.Verify(r => r.GetRoleIdByRoleNameAsync(It.IsAny<string>()), Times.Never);
        _mockConRepo.Verify(r => r.GetConstituencyIdByNameAsync(It.IsAny<string>()), Times.Never);
        _mockUserRepo.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserInfoAsyncAsync_RoleNotFound_ThrowsNotFound()
    {
        // Arrange
        var existing = CreateUser(15, 2, 10, "e@test.com", "Ex");
        var vm = CreateUserVm("New", "n@test.com", 28, 123, "F", "addr", "MissingRole", "Central");

        _mockUserRepo.Setup(r => r.GetUserByIdAsync(15)).ReturnsAsync(existing);
        _mockRoleRepo.Setup(r => r.GetRoleIdByRoleNameAsync(vm.RoleName)).ReturnsAsync(0);

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _service.UpdateUserInfoAsyncAsync(15, vm));

        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
        Assert.Equal($"No Role found with name {vm.RoleName}", ex.Message);

        _mockConRepo.Verify(r => r.GetConstituencyIdByNameAsync(It.IsAny<string>()), Times.Never);
        _mockUserRepo.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        _mockMapper.Verify(m => m.Map<UserDto>(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserInfoAsyncAsync_ConstituencyNotFound_ThrowsNotFound()
    {
        // Arrange
        var existing = CreateUser(16, 2, 10, "e@test.com", "Ex");
        var vm = CreateUserVm("New", "n@test.com", 28, 123, "F", "addr", "Leader", "MissingCons");

        _mockUserRepo.Setup(r => r.GetUserByIdAsync(16)).ReturnsAsync(existing);
        _mockRoleRepo.Setup(r => r.GetRoleIdByRoleNameAsync(vm.RoleName)).ReturnsAsync(9);
        _mockConRepo.Setup(r => r.GetConstituencyIdByNameAsync(vm.ConstituencyName)).ReturnsAsync(0);

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _service.UpdateUserInfoAsyncAsync(16, vm));

        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
        Assert.Equal($"No constituency found with name {vm.ConstituencyName}", ex.Message);

        _mockUserRepo.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        _mockMapper.Verify(m => m.Map<UserDto>(It.IsAny<User>()), Times.Never);
    }

    private static User CreateUser(int id, int roleId, int constituencyId, string email, string name) =>
        new User
        {
            UserId = id,
            RoleId = roleId,
            ConstituencyId = constituencyId,
            Email = email,
            FullName = name,
            Age = 25,
            MobileNumber = 9999999,
            Gender = "M",
            Address = "Addr",
            DateCreated = DateTime.UtcNow
        };

    private static UserVm CreateUserVm(string fullName, string email, int age, long mobile, string gender, string address, string roleName, string constituencyName) =>
        new UserVm
        {
            FullName = fullName,
            Email = email,
            Age = age,
            MobileNumber = mobile,
            Gender = gender,
            Address = address,
            RoleName = roleName,
            ConstituencyName = constituencyName
        };
}
