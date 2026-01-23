namespace WelfareWorkTracker.Tests.Infrastructure.Repositories;
public class FeedbackRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly WelfareWorkTrackerContext _context;
    private readonly FeedbackRepository _repository;

    public FeedbackRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _repository = new FeedbackRepository(_context);
    }

    // AddFeedbackAsync

    [Fact]
    public async Task AddFeedbackAsync_WithValidEntity_ReturnsPersistedEntity()
    {
        // Arrange
        var f = BuildFeedback(complaintId: 10, dailyComplaintId: null, userId: 101, isSatisfied: true, when: DateTime.UtcNow);

        // Act
        var result = await _repository.AddFeedbackAsync(f);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.CitizenFeedbackId > 0);
        var fetched = await _context.ComplaintFeedbacks.FindAsync(result.CitizenFeedbackId);
        Assert.NotNull(fetched);
        Assert.Equal(10, fetched!.ComplaintId);
        Assert.Null(fetched.DailyComplaintId);
        Assert.Equal(101, fetched.CitizenId);
        Assert.True(fetched.IsSatisfied);
    }

    // GetAllFeedbacksAsync

    [Fact]
    public async Task GetAllFeedbacksAsync_WithComplaintId_ReturnsOnlyComplaintFeedbacks()
    {
        // Arrange
        var a = BuildFeedback(complaintId: 1, dailyComplaintId: null, userId: 7, isSatisfied: true, when: DateTime.UtcNow.AddMinutes(-3));
        var b = BuildFeedback(complaintId: 1, dailyComplaintId: null, userId: 8, isSatisfied: false, when: DateTime.UtcNow.AddMinutes(-2));
        var c = BuildFeedback(complaintId: 2, dailyComplaintId: null, userId: 9, isSatisfied: true, when: DateTime.UtcNow.AddMinutes(-1));
        _context.ComplaintFeedbacks.AddRange(a, b, c);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllFeedbacksAsync(complaintId: 1, dailyComplaintId: null);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, x => Assert.Equal(1, x.ComplaintId));
    }

    [Fact]
    public async Task GetAllFeedbacksAsync_WithDailyComplaintId_ReturnsOnlyDailyFeedbacks()
    {
        // Arrange
        var a = BuildFeedback(complaintId: null, dailyComplaintId: 5, userId: 7, isSatisfied: true, when: DateTime.UtcNow.AddMinutes(-3));
        var b = BuildFeedback(complaintId: null, dailyComplaintId: 5, userId: 8, isSatisfied: false, when: DateTime.UtcNow.AddMinutes(-2));
        var c = BuildFeedback(complaintId: null, dailyComplaintId: 6, userId: 9, isSatisfied: true, when: DateTime.UtcNow.AddMinutes(-1));
        _context.ComplaintFeedbacks.AddRange(a, b, c);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllFeedbacksAsync(complaintId: null, dailyComplaintId: 5);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, x => Assert.Equal(5, x.DailyComplaintId));
    }

    [Fact]
    public async Task GetAllFeedbacksAsync_WithoutAnyIds_ThrowsException()
    {
        // Arrange

        // Act & Assert
        await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.GetAllFeedbacksAsync(null, null));
    }

    // GetFeedbackByUserAsync

    [Fact]
    public async Task GetFeedbackByUserAsync_WithComplaintIdAndUser_ReturnsMatchOrNull()
    {
        // Arrange
        var wantUser = 42;
        var has = BuildFeedback(complaintId: 3, dailyComplaintId: null, userId: wantUser, isSatisfied: true, when: DateTime.UtcNow);
        var otherUser = BuildFeedback(complaintId: 3, dailyComplaintId: null, userId: 99, isSatisfied: false, when: DateTime.UtcNow);
        _context.ComplaintFeedbacks.AddRange(has, otherUser);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repository.GetFeedbackByUserAsync(wantUser, complaintId: 3, dailyComplaintId: null);
        var notFound = await _repository.GetFeedbackByUserAsync(77, complaintId: 3, dailyComplaintId: null);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(wantUser, found!.CitizenId);
        Assert.Equal(3, found.ComplaintId);
        Assert.Null(notFound);
    }

    [Fact]
    public async Task GetFeedbackByUserAsync_WithDailyComplaintIdAndUser_ReturnsMatchOrNull()
    {
        // Arrange
        var user = 55;
        var has = BuildFeedback(complaintId: null, dailyComplaintId: 9, userId: user, isSatisfied: true, when: DateTime.UtcNow);
        var otherDaily = BuildFeedback(complaintId: null, dailyComplaintId: 8, userId: user, isSatisfied: true, when: DateTime.UtcNow);
        _context.ComplaintFeedbacks.AddRange(has, otherDaily);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repository.GetFeedbackByUserAsync(user, complaintId: null, dailyComplaintId: 9);
        var notFound = await _repository.GetFeedbackByUserAsync(user, complaintId: null, dailyComplaintId: 10);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(9, found!.DailyComplaintId);
        Assert.Null(notFound);
    }

    [Fact]
    public async Task GetFeedbackByUserAsync_WithoutAnyIds_ThrowsException()
    {
        // Arrange

        // Act & Assert
        await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _repository.GetFeedbackByUserAsync(1, null, null));
    }

    // GetSatisfiedCount

    [Fact]
    public async Task GetSatisfiedCount_WithComplaintId_ReturnsCount()
    {
        // Arrange
        var a = BuildFeedback(complaintId: 7, dailyComplaintId: null, userId: 1, isSatisfied: true, when: DateTime.UtcNow);
        var b = BuildFeedback(complaintId: 7, dailyComplaintId: null, userId: 2, isSatisfied: false, when: DateTime.UtcNow);
        var c = BuildFeedback(complaintId: 7, dailyComplaintId: null, userId: 3, isSatisfied: true, when: DateTime.UtcNow);
        var d = BuildFeedback(complaintId: 8, dailyComplaintId: null, userId: 4, isSatisfied: true, when: DateTime.UtcNow);
        _context.ComplaintFeedbacks.AddRange(a, b, c, d);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.GetSatisfiedCount(complaintId: 7, dailyComplaintId: null);

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetSatisfiedCount_WithDailyComplaintId_ReturnsCount()
    {
        // Arrange
        var a = BuildFeedback(complaintId: null, dailyComplaintId: 12, userId: 1, isSatisfied: true, when: DateTime.UtcNow);
        var b = BuildFeedback(complaintId: null, dailyComplaintId: 12, userId: 2, isSatisfied: false, when: DateTime.UtcNow);
        var c = BuildFeedback(complaintId: null, dailyComplaintId: 12, userId: 3, isSatisfied: true, when: DateTime.UtcNow);
        var d = BuildFeedback(complaintId: null, dailyComplaintId: 13, userId: 4, isSatisfied: true, when: DateTime.UtcNow);
        _context.ComplaintFeedbacks.AddRange(a, b, c, d);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.GetSatisfiedCount(complaintId: null, dailyComplaintId: 12);

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetSatisfiedCount_WithoutAnyIds_ReturnsZero()
    {
        // Arrange
        var f = BuildFeedback(complaintId: 99, dailyComplaintId: null, userId: 1, isSatisfied: true, when: DateTime.UtcNow);
        _context.ComplaintFeedbacks.Add(f);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.GetSatisfiedCount();

        // Assert
        Assert.Equal(0, count);
    }

    // GetUnSatisfiedCount

    [Fact]
    public async Task GetUnSatisfiedCount_WithComplaintId_ReturnsCount()
    {
        // Arrange
        var a = BuildFeedback(complaintId: 21, dailyComplaintId: null, userId: 1, isSatisfied: false, when: DateTime.UtcNow);
        var b = BuildFeedback(complaintId: 21, dailyComplaintId: null, userId: 2, isSatisfied: false, when: DateTime.UtcNow);
        var c = BuildFeedback(complaintId: 21, dailyComplaintId: null, userId: 3, isSatisfied: true, when: DateTime.UtcNow);
        var d = BuildFeedback(complaintId: 22, dailyComplaintId: null, userId: 4, isSatisfied: false, when: DateTime.UtcNow);
        _context.ComplaintFeedbacks.AddRange(a, b, c, d);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.GetUnSatisfiedCount(complaintId: 21, dailyComplaintId: null);

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetUnSatisfiedCount_WithDailyComplaintId_ReturnsCount()
    {
        // Arrange
        var a = BuildFeedback(complaintId: null, dailyComplaintId: 31, userId: 1, isSatisfied: false, when: DateTime.UtcNow);
        var b = BuildFeedback(complaintId: null, dailyComplaintId: 31, userId: 2, isSatisfied: true, when: DateTime.UtcNow);
        var c = BuildFeedback(complaintId: null, dailyComplaintId: 31, userId: 3, isSatisfied: false, when: DateTime.UtcNow);
        var d = BuildFeedback(complaintId: null, dailyComplaintId: 32, userId: 4, isSatisfied: false, when: DateTime.UtcNow);
        _context.ComplaintFeedbacks.AddRange(a, b, c, d);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.GetUnSatisfiedCount(complaintId: null, dailyComplaintId: 31);

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetUnSatisfiedCount_WithoutAnyIds_ReturnsZero()
    {
        // Arrange
        var f = BuildFeedback(complaintId: null, dailyComplaintId: 88, userId: 1, isSatisfied: false, when: DateTime.UtcNow);
        _context.ComplaintFeedbacks.Add(f);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.GetUnSatisfiedCount();

        // Assert
        Assert.Equal(0, count);
    }

    private ComplaintFeedback BuildFeedback(int? complaintId, int? dailyComplaintId, int userId, bool isSatisfied, DateTime when)
    {
        return _fixture.Build<ComplaintFeedback>()
            .With(x => x.CitizenFeedbackId, 0)
            .With(x => x.ComplaintId, complaintId)
            .With(x => x.DailyComplaintId, dailyComplaintId)
            .With(x => x.CitizenId, userId)
            .With(x => x.IsSatisfied, isSatisfied)
            .With(x => x.FeedbackMessage, "ok")
            .With(x => x.DateCreated, when)
            .With(x => x.DateUpdated, when)
            .Create();
    }
}
