namespace WelfareWorkTracker.Tests.Infrastructure.Repositories;
public class UserRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly WelfareWorkTrackerContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _repository = new UserRepository(_context);
    }

    // GetAdminAsync

    [Fact]
    public async Task GetAdminAsync_WithAdminUser_ReturnsAdmin()
    {
        // Arrange
        var admin = BuildUser(roleId: (int)Roles.Admin, constituencyId: 1, email: "admin@x.com");
        var user = BuildUser(roleId: (int)Roles.Citizen, constituencyId: 1, email: "c@x.com");
        _context.Users.AddRange(admin, user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAdminAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)Roles.Admin, result!.RoleId);
    }

    [Fact]
    public async Task GetAdminAsync_WhenNoAdmin_ReturnsNull()
    {
        // Arrange
        var user = BuildUser(roleId: (int)Roles.Citizen, constituencyId: 1, email: "c@x.com");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAdminAsync();

        // Assert
        Assert.Null(result);
    }

    // GetAdminRepByConstituencyIdAsync

    [Fact]
    public async Task GetAdminRepByConstituencyIdAsync_WithMatch_ReturnsAdminRep()
    {
        // Arrange
        var rep = BuildUser(roleId: (int)Roles.AdminRepresentative, constituencyId: 10, email: "rep@x.com");
        var other = BuildUser(roleId: (int)Roles.AdminRepresentative, constituencyId: 11, email: "rep2@x.com");
        _context.Users.AddRange(rep, other);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAdminRepByConstituencyIdAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result!.ConstituencyId);
        Assert.Equal((int)Roles.AdminRepresentative, result.RoleId);
    }

    [Fact]
    public async Task GetAdminRepByConstituencyIdAsync_WhenNoMatch_ReturnsNull()
    {
        // Arrange
        var rep = BuildUser(roleId: (int)Roles.AdminRepresentative, constituencyId: 10, email: "rep@x.com");
        _context.Users.Add(rep);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAdminRepByConstituencyIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    // GetAllLeadersAsync

    [Fact]
    public async Task GetAllLeadersAsync_WithLeaders_ReturnsList()
    {
        // Arrange
        var l1 = BuildUser((int)Roles.Leader, 1, "l1@x.com");
        var l2 = BuildUser((int)Roles.Leader, 2, "l2@x.com");
        var c1 = BuildUser((int)Roles.Citizen, 1, "c1@x.com");
        _context.Users.AddRange(l1, l2, c1);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllLeadersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
        Assert.All(result, u => Assert.Equal((int)Roles.Leader, u.RoleId));
    }

    [Fact]
    public async Task GetAllLeadersAsync_WhenNone_ReturnsEmpty()
    {
        // Arrange
        var c1 = BuildUser((int)Roles.Citizen, 1, "c1@x.com");
        _context.Users.Add(c1);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllLeadersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result!);
    }

    // GetCitizenCountInConstituencyAsync

    [Fact]
    public async Task GetCitizenCountInConstituencyAsync_WithMixedRoles_ReturnsOnlyCitizensCount()
    {
        // Arrange
        var c1 = BuildUser((int)Roles.Citizen, 5, "a@x.com");
        var c2 = BuildUser((int)Roles.Citizen, 5, "b@x.com");
        var l1 = BuildUser((int)Roles.Leader, 5, "l@x.com");
        var cOther = BuildUser((int)Roles.Citizen, 6, "c@x.com");
        _context.Users.AddRange(c1, c2, l1, cOther);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.GetCitizenCountInConstituencyAsync(5);

        // Assert
        Assert.Equal(2, count);
    }

    // GetLeaderByConstituencyIdAsync

    [Fact]
    public async Task GetLeaderByConstituencyIdAsync_WithLeader_ReturnsLeader()
    {
        // Arrange
        var leader = BuildUser((int)Roles.Leader, 8, "lead@x.com");
        var citizen = BuildUser((int)Roles.Citizen, 8, "cit@x.com");
        _context.Users.AddRange(leader, citizen);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetLeaderByConstituencyIdAsync(8);

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)Roles.Leader, result!.RoleId);
        Assert.Equal(8, result.ConstituencyId);
    }

    [Fact]
    public async Task GetLeaderByConstituencyIdAsync_WhenAbsent_ReturnsNull()
    {
        // Arrange
        var citizen = BuildUser((int)Roles.Citizen, 8, "cit@x.com");
        _context.Users.Add(citizen);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetLeaderByConstituencyIdAsync(8);

        // Assert
        Assert.Null(result);
    }

    // GetUserByEmailAsync

    [Fact]
    public async Task GetUserByEmailAsync_WithExistingEmail_ReturnsUser()
    {
        // Arrange
        var u = BuildUser((int)Roles.Citizen, 1, "find@x.com");
        _context.Users.Add(u);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserByEmailAsync("find@x.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("find@x.com", result!.Email);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var u = BuildUser((int)Roles.Citizen, 1, "a@x.com");
        _context.Users.Add(u);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserByEmailAsync("none@x.com");

        // Assert
        Assert.Null(result);
    }

    // GetUserByIdAsync

    [Fact]
    public async Task GetUserByIdAsync_WithExistingId_ReturnsUser()
    {
        // Arrange
        var u = BuildUser((int)Roles.Citizen, 2, "id@x.com");
        _context.Users.Add(u);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserByIdAsync(u.UserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(u.UserId, result!.UserId);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithMissingId_ReturnsNull()
    {
        // Arrange
        var u = BuildUser((int)Roles.Citizen, 2, "id@x.com");
        _context.Users.Add(u);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserByIdAsync(u.UserId + 1000);

        // Assert
        Assert.Null(result);
    }

    // GetUsersByConstituencyIdAsync

    [Fact]
    public async Task GetUsersByConstituencyIdAsync_WithCitizensOnly_ReturnsCitizens()
    {
        // Arrange
        var c1 = BuildUser((int)Roles.Citizen, 12, "c1@x.com");
        var c2 = BuildUser((int)Roles.Citizen, 12, "c2@x.com");
        var l1 = BuildUser((int)Roles.Leader, 12, "l1@x.com");
        _context.Users.AddRange(c1, c2, l1);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUsersByConstituencyIdAsync(12);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
        Assert.All(result, u => Assert.Equal((int)Roles.Citizen, u.RoleId));
    }

    [Fact]
    public async Task GetUsersByConstituencyIdAsync_WhenNoCitizens_ReturnsEmpty()
    {
        // Arrange
        var l1 = BuildUser((int)Roles.Leader, 12, "l1@x.com");
        _context.Users.Add(l1);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUsersByConstituencyIdAsync(12);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result!);
    }

    // UpdateLeaderReputationAsync

    [Fact]
    public async Task UpdateLeaderReputationAsync_WithExistingLeader_UpdatesAndReturns()
    {
        // Arrange
        var leader = BuildUser((int)Roles.Leader, 3, "leader@x.com");
        leader.Reputation = 1.0;
        _context.Users.Add(leader);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.UpdateLeaderReputationAsync(leader.UserId, 4.5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4.5, result.Reputation);
        var fetched = await _context.Users.FindAsync(leader.UserId);
        Assert.Equal(4.5, fetched!.Reputation);
    }

    [Fact]
    public async Task UpdateLeaderReputationAsync_WhenLeaderMissing_ThrowsNullReference()
    {
        // Arrange

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _repository.UpdateLeaderReputationAsync(9999, 3.2));
    }

    // UpdateUserAsync

    [Fact]
    public async Task UpdateUserAsync_WithTrackedEntity_PersistsChanges()
    {
        // Arrange
        var u = BuildUser((int)Roles.Citizen, 20, "upd@x.com");
        _context.Users.Add(u);
        await _context.SaveChangesAsync();

        u.FullName = "Updated Name";
        u.IsActive = false;
        u.DateUpdated = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateUserAsync(u);

        // Assert
        Assert.NotNull(result);
        var fetched = await _context.Users.FindAsync(u.UserId);
        Assert.Equal("Updated Name", fetched!.FullName);
        Assert.False(fetched.IsActive);
    }

    [Fact]
    public async Task UpdateUserAsync_WithDetachedEntity_PersistsChanges()
    {
        // Arrange
        var existing = BuildUser((int)Roles.Citizen, 21, "det@x.com");
        _context.Users.Add(existing);
        await _context.SaveChangesAsync();
        _context.Entry(existing).State = EntityState.Detached;

        var detached = new User
        {
            UserId = existing.UserId,
            FullName = "Detached Name",
            Email = existing.Email,
            Age = existing.Age,
            MobileNumber = existing.MobileNumber,
            Gender = existing.Gender,
            Address = existing.Address,
            ConstituencyId = existing.ConstituencyId,
            PasswordHash = existing.PasswordHash,
            PasswordSalt = existing.PasswordSalt,
            RefreshToken = existing.RefreshToken,
            RefreshTokenExpiry = existing.RefreshTokenExpiry,
            IsBlacklisted = existing.IsBlacklisted,
            IsActive = true,
            RoleId = existing.RoleId,
            DateCreated = existing.DateCreated,
            DateUpdated = DateTime.UtcNow,
            Reputation = 2.0
        };

        // Act
        var result = await _repository.UpdateUserAsync(detached);

        // Assert
        Assert.NotNull(result);
        var fetched = await _context.Users.FindAsync(existing.UserId);
        Assert.Equal("Detached Name", fetched!.FullName);
        Assert.True(fetched.IsActive);
        Assert.Equal(2.0, fetched.Reputation);
    }

    private User BuildUser(int roleId, int constituencyId, string email)
    {
        var now = DateTime.UtcNow;
        return _fixture.Build<User>()
            .With(u => u.UserId, 0)
            .With(u => u.FullName, "Test User")
            .With(u => u.Email, email)
            .With(u => u.Age, 30)
            .With(u => u.MobileNumber, 9000000000)
            .With(u => u.Gender, "M")
            .With(u => u.Address, "Addr")
            .With(u => u.ConstituencyId, constituencyId)
            .With(u => u.PasswordHash, "hash")
            .With(u => u.PasswordSalt, "salt")
            .With(u => u.RefreshToken, (string?)null)
            .With(u => u.RefreshTokenExpiry, (DateTime?)null)
            .With(u => u.IsBlacklisted, false)
            .With(u => u.IsActive, true)
            .With(u => u.RoleId, roleId)
            .With(u => u.DateCreated, now)
            .With(u => u.DateUpdated, now)
            .With(u => u.Reputation, 100.0)
            .Create();
    }
}
