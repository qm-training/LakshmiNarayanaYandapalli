namespace WelfareWorkTrackerAuth.Tests.Infrastructure.Repositories;
public class RoleRepositoryTests
{
    private readonly WelfareWorkTrackerContext _context;
    private readonly RoleRepository _roleRepository;

    public RoleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _roleRepository = new RoleRepository(_context);
    }

    [Fact]
    public async Task AddRoleAsync_WithValidRole_ReturnsAddedRole()
    {
        // Arrange
        var role = new Role
        {
            RoleName = "Admin"
        };

        // Act
        var result = await _roleRepository.AddRoleAsync(role);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.RoleId > 0);
        Assert.Equal("Admin", result.RoleName);

        var roleInDb = await _context.Roles.FindAsync(result.RoleId);
        Assert.NotNull(roleInDb);
        Assert.Equal("Admin", roleInDb!.RoleName);
    }

    [Fact]
    public async Task AddRoleAsync_WithDisposedContext_ThrowsObjectDisposedException()
    {
        // Arrange
        var role = new Role
        {
            RoleName = "Manager"
        };
        await _context.DisposeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            await _roleRepository.AddRoleAsync(role));
    }

    [Fact]
    public async Task GetRolesAsync_WithExistingRoles_ReturnsRoles()
    {
        // Arrange
        var roles = new List<Role>
        {
            new Role { RoleName = "Admin" },
            new Role { RoleName = "User" }
        };

        await _context.Roles.AddRangeAsync(roles);
        await _context.SaveChangesAsync();

        // Act
        var result = await _roleRepository.GetRolesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.RoleName == "Admin");
        Assert.Contains(result, r => r.RoleName == "User");
    }

    [Fact]
    public async Task GetRolesAsync_WithNoRoles_ReturnsEmptyList()
    {
        // Arrange
        // No roles added to context

        // Act
        var result = await _roleRepository.GetRolesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
