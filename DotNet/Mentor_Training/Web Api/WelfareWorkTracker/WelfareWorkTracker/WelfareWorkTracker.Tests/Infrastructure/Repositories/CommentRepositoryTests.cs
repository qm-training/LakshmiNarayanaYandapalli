namespace WelfareWorkTracker.Tests.Infrastructure.Repositories;
public class CommentRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly WelfareWorkTrackerContext _context;
    private readonly CommentRepository _repository;

    public CommentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _repository = new CommentRepository(_context);
    }

    // AddCommentByIdAsync

    [Fact]
    public async Task AddCommentByIdAsync_WithValidComment_ReturnsPersistedComment()
    {
        // Arrange
        var comment = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.DateCreated, DateTime.UtcNow)
            .Create();

        // Act
        var result = await _repository.AddCommentByIdAsync(comment);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.CommentId > 0);
        var fetched = await _context.Comments.FindAsync(result.CommentId);
        Assert.NotNull(fetched);
    }

    [Fact]
    public async Task AddCommentByIdAsync_WithMinimalFields_ReturnsPersistedComment()
    {
        // Arrange
        var comment = new Comment
        {
            CommentId = 0,
            UserId = 1,
            ComplaintId = 2,
            DateCreated = DateTime.UtcNow,
            Description = "x"
        };

        // Act
        var result = await _repository.AddCommentByIdAsync(comment);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.CommentId > 0);
    }

    // GetCommentByIdAsync

    [Fact]
    public async Task GetCommentByIdAsync_WithExistingId_ReturnsComment()
    {
        // Arrange
        var existing = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.DateCreated, DateTime.UtcNow)
            .Create();
        _context.Comments.Add(existing);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCommentByIdAsync(existing.CommentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existing.CommentId, result!.CommentId);
    }

    [Fact]
    public async Task GetCommentByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var nonExistingId = 9999;

        // Act
        var result = await _repository.GetCommentByIdAsync(nonExistingId);

        // Assert
        Assert.Null(result);
    }

    // GetCommentsByIdAsync

    [Fact]
    public async Task GetCommentsByIdAsync_WithComplaintId_FiltersAndOrdersByUserThenDate()
    {
        // Arrange
        var userId = 42;
        var complaintId = 7;

        var othersNewer = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.ComplaintId, complaintId)
            .With(c => c.UserId, 99)
            .With(c => c.DateCreated, DateTime.UtcNow.AddMinutes(5))
            .Create();

        var mineOlder = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.ComplaintId, complaintId)
            .With(c => c.UserId, userId)
            .With(c => c.DateCreated, DateTime.UtcNow.AddMinutes(-5))
            .Create();

        var mineNewest = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.ComplaintId, complaintId)
            .With(c => c.UserId, userId)
            .With(c => c.DateCreated, DateTime.UtcNow.AddMinutes(10))
            .Create();

        var differentComplaint = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.ComplaintId, complaintId + 1)
            .With(c => c.UserId, userId)
            .With(c => c.DateCreated, DateTime.UtcNow.AddMinutes(20))
            .Create();

        _context.Comments.AddRange(othersNewer, mineOlder, mineNewest, differentComplaint);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCommentsByIdAsync(complaintId, null, userId);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, c => Assert.Equal(complaintId, c.ComplaintId));
        var firstBlock = result.TakeWhile(c => c.UserId == userId).ToList();
        var secondBlock = result.Skip(firstBlock.Count).ToList();
        Assert.All(firstBlock, c => Assert.Equal(userId, c.UserId));
        Assert.All(secondBlock, c => Assert.NotEqual(userId, c.UserId));
        var myDates = firstBlock.Select(c => c.DateCreated).ToList();
        var sortedMyDates = myDates.OrderByDescending(d => d).ToList();
        Assert.True(myDates.SequenceEqual(sortedMyDates));
    }

    [Fact]
    public async Task GetCommentsByIdAsync_WithDailyComplaintId_FiltersAndOrdersByUserThenDate()
    {
        // Arrange
        var userId = 5;
        var dailyComplaintId = 111;

        var mineNew = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.DailyComplaintId, dailyComplaintId)
            .With(c => c.UserId, userId)
            .With(c => c.DateCreated, DateTime.UtcNow.AddMinutes(1))
            .Create();

        var mineOld = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.DailyComplaintId, dailyComplaintId)
            .With(c => c.UserId, userId)
            .With(c => c.DateCreated, DateTime.UtcNow.AddMinutes(-1))
            .Create();

        var others = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.DailyComplaintId, dailyComplaintId)
            .With(c => c.UserId, 77)
            .With(c => c.DateCreated, DateTime.UtcNow.AddMinutes(2))
            .Create();

        _context.Comments.AddRange(mineNew, mineOld, others);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCommentsByIdAsync(null, dailyComplaintId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, c => Assert.Equal(dailyComplaintId, c.DailyComplaintId));
        var firstBlock = result.TakeWhile(c => c.UserId == userId).ToList();
        Assert.True(firstBlock.Count >= 2);
        var myDates = firstBlock.Select(c => c.DateCreated).ToList();
        var sortedMyDates = myDates.OrderByDescending(d => d).ToList();
        Assert.True(myDates.SequenceEqual(sortedMyDates));
    }

    [Fact]
    public async Task GetCommentsByIdAsync_WithNoIdsProvided_ReturnsAllOrderedByUserThenDate()
    {
        // Arrange
        var userId = 9;

        var mine = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.UserId, userId)
            .With(c => c.DateCreated, DateTime.UtcNow.AddMinutes(3))
            .Create();

        var other1 = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.UserId, userId + 1)
            .With(c => c.DateCreated, DateTime.UtcNow.AddMinutes(4))
            .Create();

        var other2 = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.UserId, userId + 2)
            .With(c => c.DateCreated, DateTime.UtcNow.AddMinutes(2))
            .Create();

        _context.Comments.AddRange(mine, other1, other2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCommentsByIdAsync(null, null, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        var firstBlock = result.TakeWhile(c => c.UserId == userId).ToList();
        Assert.True(firstBlock.Count == 1);
    }

    // UpdateCommentByIdAsync

    [Fact]
    public async Task UpdateCommentByIdAsync_WithExistingComment_ReturnsUpdatedEntity()
    {
        // Arrange
        var existing = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.DateCreated, DateTime.UtcNow.AddMinutes(-30))
            .Create();
        _context.Comments.Add(existing);
        await _context.SaveChangesAsync();

        existing.Description = "updated";
        existing.DateUpdated = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateCommentByIdAsync(existing);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("updated", result.Description);
        var fetched = await _context.Comments.FindAsync(existing.CommentId);
        Assert.Equal("updated", fetched!.Description);
    }

    [Fact]
    public async Task UpdateCommentByIdAsync_WithDetachedEntity_TracksAndSavesChanges()
    {
        // Arrange
        var existing = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.DateCreated, DateTime.UtcNow.AddMinutes(-60))
            .Create();
        _context.Comments.Add(existing);
        await _context.SaveChangesAsync();

        _context.Entry(existing).State = EntityState.Detached;

        var detached = new Comment
        {
            CommentId = existing.CommentId,
            UserId = existing.UserId,
            ComplaintId = existing.ComplaintId,
            DailyComplaintId = existing.DailyComplaintId,
            Description = "detached-update",
            DateCreated = existing.DateCreated,
            DateUpdated = DateTime.UtcNow
        };

        // Act
        var result = await _repository.UpdateCommentByIdAsync(detached);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("detached-update", result.Description);
        var fetched = await _context.Comments.FindAsync(existing.CommentId);
        Assert.Equal("detached-update", fetched!.Description);
    }

    // DeleteCommentAsync

    [Fact]
    public async Task DeleteCommentAsync_WithExistingComment_RemovesAndReturnsEntity()
    {
        // Arrange
        var existing = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.DateCreated, DateTime.UtcNow)
            .Create();
        _context.Comments.Add(existing);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteCommentAsync(existing);

        // Assert
        Assert.NotNull(result);
        var deleted = await _context.Comments.FindAsync(existing.CommentId);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteCommentAsync_WithDetachedEntity_RemovesByKey()
    {
        // Arrange
        var existing = _fixture.Build<Comment>()
            .With(c => c.CommentId, 0)
            .With(c => c.DateCreated, DateTime.UtcNow)
            .Create();
        _context.Comments.Add(existing);
        await _context.SaveChangesAsync();

        _context.Entry(existing).State = EntityState.Detached;

        var toRemove = new Comment { CommentId = existing.CommentId };

        // Act
        var result = await _repository.DeleteCommentAsync(toRemove);

        // Assert
        Assert.NotNull(result);
        var deleted = await _context.Comments.FindAsync(existing.CommentId);
        Assert.Null(deleted);
    }
}
