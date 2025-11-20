namespace WelfareWorkTracker.Tests.Infrastructure.Repositories;
public class RoleRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly WelfareWorkTrackerContext _context;
    private readonly RoleRepository _repository;

    public RoleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _repository = new RoleRepository(_context);
    }

    // AddRoleAsync

    [Fact]
    public async Task AddRoleAsync_WithValidRole_PersistsAndReturnsRole()
    {
        // Arrange
        var role = BuildRole("Admin");

        // Act
        var result = await _repository.AddRoleAsync(role);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.RoleId > 0);
        Assert.Equal("Admin", result.RoleName);
        var fetched = await _context.Roles.FindAsync(result.RoleId);
        Assert.NotNull(fetched);
        Assert.Equal("Admin", fetched!.RoleName);
    }

    [Fact]
    public async Task AddRoleAsync_MissingRoleName_ThrowsDbUpdateException()
    {
        // Arrange
        var role = new Role { RoleName = null! };

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddRoleAsync(role));
    }

    // GetAllRolesAsync

    [Fact]
    public async Task GetAllRolesAsync_WhenRolesExist_ReturnsAll()
    {
        // Arrange
        var admin = BuildRole("Admin");
        var user = BuildRole("User");
        _context.Roles.AddRange(admin, user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllRolesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.RoleName == "Admin");
        Assert.Contains(result, r => r.RoleName == "User");
    }

    [Fact]
    public async Task GetAllRolesAsync_WhenNoRoles_ReturnsEmptyList()
    {
        // Arrange

        // Act
        var result = await _repository.GetAllRolesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // GetRoleIdByRoleNameAsync

    [Fact]
    public async Task GetRoleIdByRoleNameAsync_WithExistingRole_ReturnsCorrectId()
    {
        // Arrange
        var manager = BuildRole("Manager");
        _context.Roles.Add(manager);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRoleIdByRoleNameAsync("Manager");

        // Assert
        Assert.Equal(manager.RoleId, result);
    }

    [Fact]
    public async Task GetRoleIdByRoleNameAsync_WithNonExistingRole_ReturnsZero()
    {
        // Arrange
        var role = BuildRole("Employee");
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRoleIdByRoleNameAsync("Unknown");

        // Assert
        Assert.Equal(0, result);
    }

    private Role BuildRole(string name)
    {
        return _fixture.Build<Role>()
            .With(r => r.RoleId, 0)
            .With(r => r.RoleName, name)
            .Create();
    }
}
