namespace WelfareWorkTrackerAuth.Tests.Infrastructure.Repositories;
public class UserRepositoryTests
{
    private readonly WelfareWorkTrackerContext _context;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _userRepository = new UserRepository(_context);
    }

    [Fact]
    public async Task CheckIfUserEmailExistsAsync_WithExistingEmail_ReturnsUser()
    {
        // Arrange
        var email = "test.user@example.com";
        var createdUser = await CreateUserAsync(email: email);

        // Act
        var result = await _userRepository.CheckIfUserEmailExistsAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdUser.UserId, result!.UserId);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task CheckIfUserEmailExistsAsync_WithNonExistingEmail_ReturnsNull()
    {
        // Arrange
        await CreateUserAsync(email: "some.other@example.com");
        var nonExistingEmail = "not.present@example.com";

        // Act
        var result = await _userRepository.CheckIfUserEmailExistsAsync(nonExistingEmail);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithExistingId_ReturnsUser()
    {
        // Arrange
        var createdUser = await CreateUserAsync();
        var userId = createdUser.UserId;

        // Act
        var result = await _userRepository.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result!.UserId);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var nonExistingUserId = 999;

        // Act
        var result = await _userRepository.GetUserByIdAsync(nonExistingUserId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterUserAsync_WithValidUser_ReturnsTrueAndPersistsUser()
    {
        // Arrange
        var user = new User
        {
            FullName = "Register User",
            Email = "register.user@example.com",
            Age = 25,
            MobileNumber = 9876543210,
            Gender = "Other",
            Address = "Register Address",
            ConstituencyId = 1,
            PasswordHash = "hash",
            PasswordSalt = "salt",
            RefreshToken = null,
            RefreshTokenExpiry = null,
            IsBlacklisted = false,
            IsActive = true,
            RoleId = 2,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            Reputation = 0
        };

        // Act
        var result = await _userRepository.RegisterUserAsync(user);

        // Assert
        Assert.True(result);
        var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        Assert.NotNull(userInDb);
        Assert.Equal("Register User", userInDb!.FullName);
    }

    [Fact]
    public async Task RegisterUserAsync_WithDisposedContext_ThrowsObjectDisposedException()
    {
        // Arrange
        var user = new User
        {
            FullName = "Disposed User",
            Email = "disposed.user@example.com",
            Age = 30,
            MobileNumber = 1112223333,
            Gender = "Male",
            Address = "Disposed Address",
            ConstituencyId = 1,
            PasswordHash = "hash",
            PasswordSalt = "salt",
            RefreshToken = null,
            RefreshTokenExpiry = null,
            IsBlacklisted = false,
            IsActive = true,
            RoleId = 1,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            Reputation = 0
        };

        await _context.DisposeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            await _userRepository.RegisterUserAsync(user));
    }

    [Fact]
    public async Task UpdatePasswordAsync_WithExistingUser_UpdatesPasswordAndReturnsTrue()
    {
        // Arrange
        var existingUser = await CreateUserAsync();
        var userId = existingUser.UserId;
        var newSalt = "new-salt";
        var newHash = "new-hash";

        // Act
        var result = await _userRepository.UpdatePasswordAsync(userId, newSalt, newHash);

        // Assert
        Assert.True(result);
        var userInDb = await _context.Users.FindAsync(userId);
        Assert.NotNull(userInDb);
        Assert.Equal(newSalt, userInDb!.PasswordSalt);
        Assert.Equal(newHash, userInDb.PasswordHash);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WithNonExistingUser_ReturnsFalse()
    {
        // Arrange
        var nonExistingUserId = 999;
        var newSalt = "salt";
        var newHash = "hash";

        // Act
        var result = await _userRepository.UpdatePasswordAsync(nonExistingUserId, newSalt, newHash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateUserAsync_WithExistingUser_ReturnsTrueAndPersistsChanges()
    {
        // Arrange
        var existingUser = await CreateUserAsync();
        existingUser.FullName = "Updated Name";
        existingUser.Address = "Updated Address";

        // Act
        var result = await _userRepository.UpdateUserAsync(existingUser);

        // Assert
        Assert.True(result);
        var userInDb = await _context.Users.FindAsync(existingUser.UserId);
        Assert.NotNull(userInDb);
        Assert.Equal("Updated Name", userInDb!.FullName);
        Assert.Equal("Updated Address", userInDb.Address);
    }

    [Fact]
    public async Task UpdateUserAsync_WithDisposedContext_ThrowsObjectDisposedException()
    {
        // Arrange
        var existingUser = await CreateUserAsync();
        existingUser.FullName = "Should Not Update";
        await _context.DisposeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            await _userRepository.UpdateUserAsync(existingUser));
    }

    private async Task<User> CreateUserAsync(
        string? email = null,
        string? fullName = null)
    {
        var user = new User
        {
            FullName = fullName ?? "Test User",
            Email = email ?? "test.user@example.com",
            Age = 30,
            MobileNumber = 1234567890,
            Gender = "Male",
            Address = "Test Address",
            ConstituencyId = 1,
            PasswordHash = "hash",
            PasswordSalt = "salt",
            RefreshToken = null,
            RefreshTokenExpiry = null,
            IsBlacklisted = false,
            IsActive = true,
            RoleId = 1,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            Reputation = 0
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
}
