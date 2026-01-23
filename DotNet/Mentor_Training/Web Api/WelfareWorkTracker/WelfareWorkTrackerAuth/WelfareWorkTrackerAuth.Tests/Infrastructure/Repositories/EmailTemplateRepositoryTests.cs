namespace WelfareWorkTrackerAuth.Tests.Infrastructure.Repositories;
public class EmailTemplateRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly WelfareWorkTrackerContext _context;
    private readonly EmailTemplateRepository _repository;

    public EmailTemplateRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _repository = new EmailTemplateRepository(_context);
    }

    // GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WithExistingActiveId_ReturnsTemplate()
    {
        // Arrange
        var active = BuildTemplate("Welcome", isActive: true);
        var inactive = BuildTemplate("Old", isActive: false);
        _context.EmailTemplates.AddRange(active, inactive);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(active.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(active.Id, result.Id);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingInactiveId_ThrowsNotFound()
    {
        // Arrange
        var inactive = BuildTemplate("Legacy", isActive: false);
        _context.EmailTemplates.Add(inactive);
        await _context.SaveChangesAsync();

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.GetByIdAsync(inactive.Id));

        // Assert
        Assert.Contains($"Email Template with Id {inactive.Id} Not Found!", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ThrowsNotFound()
    {
        // Arrange

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.GetByIdAsync(9999));

        // Assert
        Assert.Contains("Email Template with Id 9999 Not Found!", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }

    // GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActive()
    {
        // Arrange
        var a = BuildTemplate("A", true);
        var b = BuildTemplate("B", true);
        var c = BuildTemplate("C", false);
        _context.EmailTemplates.AddRange(a, b, c);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.All(result, t => Assert.True(t.IsActive));
        Assert.Contains(result, t => t.Id == a.Id);
        Assert.Contains(result, t => t.Id == b.Id);
        Assert.DoesNotContain(result, t => t.Id == c.Id);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoActive_ReturnsEmpty()
    {
        // Arrange
        var c = BuildTemplate("C", false);
        _context.EmailTemplates.Add(c);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // GetByNameAsync

    [Fact]
    public async Task GetByNameAsync_WithActiveName_ReturnsTemplate()
    {
        // Arrange
        var t = BuildTemplate("Receipt", true);
        _context.EmailTemplates.Add(t);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync("Receipt");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(t.Id, result.Id);
        Assert.Equal("Receipt", result.Name);
    }

    [Fact]
    public async Task GetByNameAsync_WhenInactive_ThrowsNotFound()
    {
        // Arrange
        var t = BuildTemplate("Promo", false);
        _context.EmailTemplates.Add(t);
        await _context.SaveChangesAsync();

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.GetByNameAsync("Promo"));

        // Assert
        Assert.Contains("Email Template with template name Promo Not Found!", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetByNameAsync_WhenMissing_ThrowsNotFound()
    {
        // Arrange

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.GetByNameAsync("Missing"));

        // Assert
        Assert.Contains("Email Template with template name Missing Not Found!", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }

    // AddAsync

    [Fact]
    public async Task AddAsync_WithValidEntity_ReturnsTrueAndPersists()
    {
        // Arrange
        var t = BuildTemplate("AddMe", true);

        // Act
        var ok = await _repository.AddAsync(t);

        // Assert
        Assert.True(ok);
        Assert.True(t.Id > 0);
        var fetched = await _context.EmailTemplates.FindAsync(t.Id);
        Assert.NotNull(fetched);
        Assert.Equal("AddMe", fetched!.Name);
    }

    [Fact]
    public async Task AddAsync_MissingRequiredName_ThrowsDbUpdateException()
    {
        // Arrange
        var t = new EmailTemplate
        {
            Id = 0,
            Name = null!,
            Subject = "Subj",
            Body = "Body",
            IsActive = true,
            DateCreated = DateTime.UtcNow,
            DateUpdated = null,
            CreatedBy = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(t));
    }

    [Fact]
    public async Task AddAsync_MissingRequiredSubjectOrBody_ThrowsDbUpdateException()
    {
        // Arrange
        var a = new EmailTemplate
        {
            Id = 0,
            Name = "N",
            Subject = null!,
            Body = "B",
            IsActive = true,
            DateCreated = DateTime.UtcNow,
            CreatedBy = 1
        };
        var b = new EmailTemplate
        {
            Id = 0,
            Name = "N",
            Subject = "S",
            Body = null!,
            IsActive = true,
            DateCreated = DateTime.UtcNow,
            CreatedBy = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(a));
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(b));
    }

    // UpdateAsync

    [Fact]
    public async Task UpdateAsync_WithExistingTrackedEntity_ReturnsTrueAndSavesNewValues()
    {
        // Arrange
        var existing = BuildTemplate("Base", true);
        _context.EmailTemplates.Add(existing);
        await _context.SaveChangesAsync();

        existing.Subject = "NewSubj";
        existing.Body = "NewBody";
        existing.DateUpdated = DateTime.UtcNow;

        // Act
        var ok = await _repository.UpdateAsync(existing);

        // Assert
        Assert.True(ok);
        var fetched = await _context.EmailTemplates.FindAsync(existing.Id);
        Assert.NotNull(fetched);
        Assert.Equal("NewSubj", fetched!.Subject);
        Assert.Equal("NewBody", fetched.Body);
        Assert.Equal(existing.DateUpdated, fetched.DateUpdated);
    }

    [Fact]
    public async Task UpdateAsync_WithDifferentInstanceSameId_ThrowsTrackingConflict()
    {
        // Arrange
        var existing = BuildTemplate("Orig", true);
        _context.EmailTemplates.Add(existing);
        await _context.SaveChangesAsync();

        var detached = new EmailTemplate
        {
            Id = existing.Id,
            Name = existing.Name,
            Subject = "DetachedSubj",
            Body = "DetachedBody",
            IsActive = existing.IsActive,
            DateCreated = existing.DateCreated,
            DateUpdated = DateTime.UtcNow,
            CreatedBy = existing.CreatedBy
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.UpdateAsync(detached));
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingId_ThrowsNotFound()
    {
        // Arrange
        var missing = BuildTemplate("X", true);
        missing.Id = 9999;

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.UpdateAsync(missing));

        // Assert
        Assert.Contains("Email Templates with Id 9999 Not Found!", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }

    // DeleteAsync

    [Fact]
    public async Task DeleteAsync_WithExistingId_RemovesAndReturnsTrue()
    {
        // Arrange
        var t = BuildTemplate("Del", true);
        _context.EmailTemplates.Add(t);
        await _context.SaveChangesAsync();

        // Act
        var ok = await _repository.DeleteAsync(t.Id);

        // Assert
        Assert.True(ok);
        var gone = await _context.EmailTemplates.FindAsync(t.Id);
        Assert.Null(gone);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ThrowsNotFound()
    {
        // Arrange

        // Act
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.DeleteAsync(424242));

        // Assert
        Assert.Contains("Email Templates with Id 424242 Not Found!", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }

    private EmailTemplate BuildTemplate(string name, bool isActive)
    {
        var now = DateTime.UtcNow;
        return _fixture.Build<EmailTemplate>()
            .With(t => t.Id, 0)
            .With(t => t.Name, name)
            .With(t => t.Subject, $"{name} Subject")
            .With(t => t.Body, $"{name} Body")
            .With(t => t.IsActive, isActive)
            .With(t => t.DateCreated, now)
            .With(t => t.DateUpdated, (DateTime?)null)
            .With(t => t.CreatedBy, 1)
            .Create();
    }
}

