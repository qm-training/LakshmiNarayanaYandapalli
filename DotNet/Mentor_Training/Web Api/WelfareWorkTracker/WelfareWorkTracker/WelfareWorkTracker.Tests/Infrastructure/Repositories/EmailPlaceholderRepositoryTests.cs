namespace WelfareWorkTracker.Tests.Infrastructure.Repositories;
public class EmailPlaceholderRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly WelfareWorkTrackerContext _context;
    private readonly EmailPlaceholderRepository _repository;

    public EmailPlaceholderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _repository = new EmailPlaceholderRepository(_context);
    }

    // GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsEntity()
    {
        // Arrange
        var p = BuildPlaceholder(1, "K1", "V1", DateTime.UtcNow);
        _context.EmailPlaceholders.Add(p);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(p.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(p.Id, result.Id);
        Assert.Equal("K1", result.PlaceHolderKey);
        Assert.Equal("V1", result.PlaceHolderValue);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ThrowsNotFound()
    {
        // Arrange

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.GetByIdAsync(9999));

        // Assert
        Assert.Contains("Email Placeholder with Id 9999 was not found!", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }

    // GetAllAsync

    [Fact]
    public async Task GetAllAsync_WithData_ReturnsAll()
    {
        // Arrange
        var a = BuildPlaceholder(10, "A", "1", DateTime.UtcNow.AddMinutes(-2));
        var b = BuildPlaceholder(10, "B", "2", DateTime.UtcNow.AddMinutes(-1));
        _context.EmailPlaceholders.AddRange(a, b);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count >= 2);
        Assert.Contains(result, x => x.Id == a.Id);
        Assert.Contains(result, x => x.Id == b.Id);
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // GetByEmailOutboxIdAsync

    [Fact]
    public async Task GetByEmailOutboxIdAsync_WithMatches_ReturnsOnlyThose()
    {
        // Arrange
        var outboxId = 50;
        var m1 = BuildPlaceholder(outboxId, "FirstName", "Alice", DateTime.UtcNow);
        var m2 = BuildPlaceholder(outboxId, "OrderId", "123", DateTime.UtcNow);
        var other = BuildPlaceholder(99, "Ignore", "X", DateTime.UtcNow);
        _context.EmailPlaceholders.AddRange(m1, m2, other);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailOutboxIdAsync(outboxId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal(outboxId, r.EmailOutboxId));
    }

    [Fact]
    public async Task GetByEmailOutboxIdAsync_WithNoMatches_ReturnsEmpty()
    {
        // Arrange
        var other = BuildPlaceholder(99, "Ignore", "X", DateTime.UtcNow);
        _context.EmailPlaceholders.Add(other);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailOutboxIdAsync(123);

        // Assert
        Assert.Empty(result);
    }

    // AddAsync

    [Fact]
    public async Task AddAsync_WithValidEntity_ReturnsTrueAndPersists()
    {
        // Arrange
        var p = BuildPlaceholder(7, "Email", "user@example.com", DateTime.UtcNow);

        // Act
        var ok = await _repository.AddAsync(p);

        // Assert
        Assert.True(ok);
        Assert.True(p.Id > 0);
        var fetched = await _context.EmailPlaceholders.FindAsync(p.Id);
        Assert.NotNull(fetched);
        Assert.Equal("Email", fetched!.PlaceHolderKey);
        Assert.Equal("user@example.com", fetched.PlaceHolderValue);
    }

    [Fact]
    public async Task AddAsync_MissingRequiredKey_ThrowsDbUpdateException()
    {
        // Arrange
        var p = new EmailPlaceholder
        {
            Id = 0,
            EmailOutboxId = 1,
            PlaceHolderKey = null!,      // required
            PlaceHolderValue = "V",
            DateCreated = DateTime.UtcNow,
            DateUpdated = null
        };

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(p));
    }

    [Fact]
    public async Task AddAsync_MissingRequiredValue_ThrowsDbUpdateException()
    {
        // Arrange
        var p = new EmailPlaceholder
        {
            Id = 0,
            EmailOutboxId = 1,
            PlaceHolderKey = "K",
            PlaceHolderValue = null!,    // required
            DateCreated = DateTime.UtcNow,
            DateUpdated = null
        };

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(p));
    }

    // UpdateAsync

    [Fact]
    public async Task UpdateAsync_WithExistingId_ReturnsTrueAndSavesNewValues()
    {
        // Arrange
        var existing = BuildPlaceholder(11, "K", "Old", DateTime.UtcNow.AddMinutes(-5));
        _context.EmailPlaceholders.Add(existing);
        await _context.SaveChangesAsync();

        existing.PlaceHolderValue = "New";
        existing.DateUpdated = DateTime.UtcNow;

        // Act
        var ok = await _repository.UpdateAsync(existing);

        // Assert
        Assert.True(ok);
        var fetched = await _context.EmailPlaceholders.FindAsync(existing.Id);
        Assert.NotNull(fetched);
        Assert.Equal("New", fetched!.PlaceHolderValue);
        Assert.Equal(existing.DateUpdated, fetched.DateUpdated);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingId_ThrowsNotFound()
    {
        // Arrange
        var input = BuildPlaceholder(5, "A", "B", DateTime.UtcNow);
        input.Id = 9999;

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.UpdateAsync(input));

        // Assert
        Assert.Contains("Email Placeholder with Id 9999 Not Found!", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }

    // DeleteAsync

    [Fact]
    public async Task DeleteAsync_WithExistingId_RemovesAndReturnsTrue()
    {
        // Arrange
        var p = BuildPlaceholder(77, "Del", "Val", DateTime.UtcNow);
        _context.EmailPlaceholders.Add(p);
        await _context.SaveChangesAsync();

        // Act
        var ok = await _repository.DeleteAsync(p.Id);

        // Assert
        Assert.True(ok);
        var gone = await _context.EmailPlaceholders.FindAsync(p.Id);
        Assert.Null(gone);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ThrowsNotFound()
    {
        // Arrange

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.DeleteAsync(424242));

        // Assert
        Assert.Contains("Email Placeholder with Id 424242 Not Found!", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }

    // AddRangeAsync

    [Fact]
    public async Task AddRangeAsync_WithValidList_ReturnsTrueAndPersistsAll()
    {
        // Arrange
        var list = new List<EmailPlaceholder>
        {
            BuildPlaceholder(100, "FirstName", "Alice", DateTime.UtcNow),
            BuildPlaceholder(100, "OrderId", "123", DateTime.UtcNow)
        };

        // Act
        var ok = await _repository.AddRangeAsync(list);

        // Assert
        Assert.True(ok);
        Assert.Equal(2, await _context.EmailPlaceholders.CountAsync());
    }

    [Fact]
    public async Task AddRangeAsync_WithEmptyList_ReturnsFalse()
    {
        // Arrange
        var list = new List<EmailPlaceholder>();

        // Act
        var ok = await _repository.AddRangeAsync(list);

        // Assert
        Assert.False(ok);
        Assert.Equal(0, await _context.EmailPlaceholders.CountAsync());
    }

    private EmailPlaceholder BuildPlaceholder(int emailOutboxId, string key, string value, DateTime created)
    {
        return _fixture.Build<EmailPlaceholder>()
            .With(x => x.Id, 0)
            .With(x => x.EmailOutboxId, emailOutboxId)
            .With(x => x.PlaceHolderKey, key)
            .With(x => x.PlaceHolderValue, value)
            .With(x => x.DateCreated, created)
            .With(x => x.DateUpdated, (DateTime?)null)
            .Create();
    }
}
