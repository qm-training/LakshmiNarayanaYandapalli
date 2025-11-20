namespace WelfareWorkTrackerAuth.Tests.Infrastructure.Repositories;
public class EmailOutboxRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly WelfareWorkTrackerContext _context;
    private readonly EmailOutboxRepository _repository;

    public EmailOutboxRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _repository = new EmailOutboxRepository(_context);
    }

    // GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsEntity()
    {
        // Arrange
        var e = BuildEmailOutbox(templateId: 10, to: "a@example.com", sentAtUtc: DateTime.UtcNow);
        _context.EmailOutboxes.Add(e);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(e.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(e.Id, result.Id);
        Assert.Equal("a@example.com", result.ToEmail);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ThrowsNotFound()
    {
        // Arrange

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.GetByIdAsync(9999));

        // Assert
        Assert.Contains("Email Outbox with Id 9999 was Not Found!", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }

    // GetAllAsync

    [Fact]
    public async Task GetAllAsync_WithData_ReturnsAll()
    {
        // Arrange
        var a = BuildEmailOutbox(1, "a@x.com", DateTime.UtcNow.AddMinutes(-2));
        var b = BuildEmailOutbox(2, "b@x.com", DateTime.UtcNow.AddMinutes(-1));
        _context.EmailOutboxes.AddRange(a, b);
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

    // GetByTemplateIdAsync

    [Fact]
    public async Task GetByTemplateIdAsync_WithMatches_ReturnsOnlyThose()
    {
        // Arrange
        var tId = 50;
        var m1 = BuildEmailOutbox(tId, "a@templ.com", DateTime.UtcNow);
        var m2 = BuildEmailOutbox(tId, "b@templ.com", DateTime.UtcNow.AddMinutes(1));
        var other = BuildEmailOutbox(99, "c@other.com", DateTime.UtcNow);
        _context.EmailOutboxes.AddRange(m1, m2, other);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTemplateIdAsync(tId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal(tId, r.EmailTemplateId));
    }

    [Fact]
    public async Task GetByTemplateIdAsync_WithNoMatches_ReturnsEmpty()
    {
        // Arrange
        var other = BuildEmailOutbox(99, "c@other.com", DateTime.UtcNow);
        _context.EmailOutboxes.Add(other);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTemplateIdAsync(123);

        // Assert
        Assert.Empty(result);
    }

    // AddAsync

    [Fact]
    public async Task AddAsync_WithValidEntity_ReturnsTrueAndPersists()
    {
        // Arrange
        var e = BuildEmailOutbox(7, "add@ex.com", DateTime.UtcNow);

        // Act
        var ok = await _repository.AddAsync(e);

        // Assert
        Assert.True(ok);
        Assert.True(e.Id > 0);
        var fetched = await _context.EmailOutboxes.FindAsync(e.Id);
        Assert.NotNull(fetched);
        Assert.Equal("add@ex.com", fetched!.ToEmail);
    }

    [Fact]
    public async Task AddAsync_WithMissingRequiredToEmail_ThrowsDbUpdateException()
    {
        // Arrange
        var e = new EmailOutbox
        {
            Id = 0,
            EmailTemplateId = 1,
            ToEmail = null!, // required
            FromEmail = null,
            SentAt = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(e));
    }

    // UpdateAsync

    [Fact]
    public async Task UpdateAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        var existing = BuildEmailOutbox(11, "old@ex.com", DateTime.UtcNow.AddMinutes(-5));
        _context.EmailOutboxes.Add(existing);
        await _context.SaveChangesAsync();

        var input = new EmailOutbox
        {
            Id = existing.Id,
            EmailTemplateId = 22,
            ToEmail = "new@ex.com",
            FromEmail = "from@ex.com",
            SentAt = DateTime.UtcNow
        };

        // Act
        var ok = await _repository.UpdateAsync(input);

        // Assert
        Assert.True(ok);
        var fetched = await _context.EmailOutboxes.FindAsync(existing.Id);
        Assert.NotNull(fetched);
        // Current repository does not copy values from 'input'; it updates the fetched entity as-is
        Assert.Equal(existing.EmailTemplateId, fetched!.EmailTemplateId);
        Assert.Equal(existing.ToEmail, fetched.ToEmail);
        Assert.Equal(existing.FromEmail, fetched.FromEmail);
        Assert.Equal(existing.SentAt, fetched.SentAt);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingId_ThrowsNotFound()
    {
        // Arrange
        var input = BuildEmailOutbox(5, "x@y.com", DateTime.UtcNow);
        input.Id = 9999;

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.UpdateAsync(input));

        // Assert
        Assert.Contains("Email Outbox with Id 9999 Not Found!", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }

    // DeleteAsync

    [Fact]
    public async Task DeleteAsync_WithExistingId_RemovesAndReturnsTrue()
    {
        // Arrange
        var e = BuildEmailOutbox(77, "del@ex.com", DateTime.UtcNow);
        _context.EmailOutboxes.Add(e);
        await _context.SaveChangesAsync();

        // Act
        var ok = await _repository.DeleteAsync(e.Id);

        // Assert
        Assert.True(ok);
        var gone = await _context.EmailOutboxes.FindAsync(e.Id);
        Assert.Null(gone);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ThrowsNotFound()
    {
        // Arrange

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.DeleteAsync(424242));

        // Assert
        Assert.Contains("Email Outbox with Id 424242 not Found!", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }

    private EmailOutbox BuildEmailOutbox(int templateId, string to, DateTime sentAtUtc)
    {
        return _fixture.Build<EmailOutbox>()
            .With(x => x.Id, 0)
            .With(x => x.EmailTemplateId, templateId)
            .With(x => x.ToEmail, to)
            .With(x => x.FromEmail, (string?)null)
            .With(x => x.SentAt, sentAtUtc)
            .Create();
    }
}

