namespace WelfareWorkTracker.Tests.Infrastructure.Repositories;
public class DailyComplaintStatusRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly WelfareWorkTrackerContext _context;
    private readonly Mock<IDailyComplaintRepository> _mockDailyComplaintRepository = new(MockBehavior.Strict);
    private readonly DailyComplaintStatusRepository _repository;

    public DailyComplaintStatusRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _repository = new DailyComplaintStatusRepository(_context, _mockDailyComplaintRepository.Object);
    }

    // AddDailyComplaintStatusAsync

    [Fact]
    public async Task AddDailyComplaintStatusAsync_WithValidEntity_ReturnsPersistedEntity()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entity = BuildDailyComplaintStatus(dailyComplaintId: 10, statusValue: 2, created: now.AddMinutes(-2), updated: now);

        // Act
        var result = await _repository.AddDailyComplaintStatusAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.DailyComplaintStatusId > 0);
        var fetched = await _context.DailyComplaintStatuses.FindAsync(result.DailyComplaintStatusId);
        Assert.NotNull(fetched);
    }

    [Fact]
    public async Task AddDailyComplaintStatusAsync_WithMinimalValidFields_PersistsAndSetsKey()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entity = new DailyComplaintStatus
        {
            DailyComplaintStatusId = 0,
            DailyComplaintId = 5,
            Status = 1,
            DateCreated = now,
            DateUpdated = now
        };

        // Act
        var result = await _repository.AddDailyComplaintStatusAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.DailyComplaintStatusId > 0);
        Assert.Equal(5, result.DailyComplaintId);
    }

    // GetDailyComplaintStatusAsync

    [Fact]
    public async Task GetDailyComplaintStatusAsync_WithDailyComplaintId_ReturnsLatestByDateUpdated()
    {
        // Arrange
        var id = 77;
        var older = BuildDailyComplaintStatus(id, statusValue: 1, created: DateTime.UtcNow.AddMinutes(-6), updated: DateTime.UtcNow.AddMinutes(-5));
        var newer = BuildDailyComplaintStatus(id, statusValue: 3, created: DateTime.UtcNow.AddMinutes(-2), updated: DateTime.UtcNow.AddMinutes(1));
        _context.DailyComplaintStatuses.AddRange(older, newer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDailyComplaintStatusAsync(dailyComplaintId: id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newer.DailyComplaintStatusId, result!.DailyComplaintStatusId);
    }

    [Fact]
    public async Task GetDailyComplaintStatusAsync_WithDailyComplaintStatusId_ReturnsExactMatch()
    {
        // Arrange
        var s1 = BuildDailyComplaintStatus(5, statusValue: 2, created: DateTime.UtcNow.AddMinutes(-3), updated: DateTime.UtcNow.AddMinutes(-2));
        var s2 = BuildDailyComplaintStatus(5, statusValue: 4, created: DateTime.UtcNow.AddMinutes(-2), updated: DateTime.UtcNow.AddMinutes(-1));
        _context.DailyComplaintStatuses.AddRange(s1, s2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDailyComplaintStatusAsync(null, s1.DailyComplaintStatusId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(s1.DailyComplaintStatusId, result!.DailyComplaintStatusId);
    }

    [Fact]
    public async Task GetDailyComplaintStatusAsync_WithoutIds_ThrowsArgumentException()
    {
        // Arrange

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetDailyComplaintStatusAsync());
    }

    // GetPendingDailyComplaintsAsync

    [Fact]
    public async Task GetPendingDailyComplaintsAsync_WhenLatestNotCompleted_ReturnsOnlyPending()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dc1_oldDone = BuildDailyComplaintStatus(1, statusValue: 8, created: now.AddMinutes(-20), updated: now.AddMinutes(-10));
        var dc1_newPending = BuildDailyComplaintStatus(1, statusValue: 5, created: now.AddMinutes(-5), updated: now.AddMinutes(-1));
        var dc2_oldPending = BuildDailyComplaintStatus(2, statusValue: 2, created: now.AddMinutes(-7), updated: now.AddMinutes(-5));
        var dc2_newCompleted = BuildDailyComplaintStatus(2, statusValue: 8, created: now.AddMinutes(-4), updated: now.AddMinutes(-2));
        var dc3_onlyCompleted = BuildDailyComplaintStatus(3, statusValue: 8, created: now.AddMinutes(-3), updated: now.AddMinutes(-3));
        _context.DailyComplaintStatuses.AddRange(dc1_oldDone, dc1_newPending, dc2_oldPending, dc2_newCompleted, dc3_onlyCompleted);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPendingDailyComplaintsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.All(result, s => Assert.NotEqual(8, s!.Status));
        Assert.Contains(result, s => s!.DailyComplaintId == 1);
        Assert.DoesNotContain(result, s => s!.DailyComplaintId == 2);
        Assert.DoesNotContain(result, s => s!.DailyComplaintId == 3);
    }

    [Fact]
    public async Task GetPendingDailyComplaintsAsync_WhenAllLatestCompleted_ReturnsEmpty()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var a = BuildDailyComplaintStatus(10, 8, now.AddMinutes(-2), now.AddMinutes(-1));
        var b = BuildDailyComplaintStatus(11, 8, now.AddMinutes(-1), now);
        _context.DailyComplaintStatuses.AddRange(a, b);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPendingDailyComplaintsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // GetUnresolvedComplaintsOfLeader

    [Fact]
    public async Task GetUnresolvedComplaintsOfLeader_WithMatches_ReturnsStatusesForLeaderAndStatus()
    {
        // Arrange
        var leaderId = 42;
        var leadersDailyComplaints = new List<DailyComplaint>
        {
            new DailyComplaint { DailyComplaintId = 1, LeaderId = leaderId, ConstituencyId = 1, IsCompleted = false, DateCreated = DateTime.UtcNow, DateUpdated = DateTime.UtcNow },
            new DailyComplaint { DailyComplaintId = 2, LeaderId = leaderId, ConstituencyId = 1, IsCompleted = false, DateCreated = DateTime.UtcNow, DateUpdated = DateTime.UtcNow }
        };
        _mockDailyComplaintRepository
            .Setup(r => r.GetDailyComplaintsForLeaderByLeaderIdAsync(leaderId))
            .ReturnsAsync(leadersDailyComplaints);

        var s1 = BuildDailyComplaintStatus(1, statusValue: (int)Status.Unresolved, created: DateTime.UtcNow.AddMinutes(-2), updated: DateTime.UtcNow.AddMinutes(-1));
        var s2 = BuildDailyComplaintStatus(2, statusValue: (int)Status.Unresolved, created: DateTime.UtcNow.AddMinutes(-2), updated: DateTime.UtcNow.AddMinutes(-1));
        var s3 = BuildDailyComplaintStatus(3, statusValue: (int)Status.Unresolved, created: DateTime.UtcNow.AddMinutes(-2), updated: DateTime.UtcNow.AddMinutes(-1));
        var s4 = BuildDailyComplaintStatus(2, statusValue: (int)Status.Resolved, created: DateTime.UtcNow.AddMinutes(-1), updated: DateTime.UtcNow);
        _context.DailyComplaintStatuses.AddRange(s1, s2, s3, s4);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUnresolvedComplaintsOfLeader(leaderId, Status.Unresolved);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, cs =>
        {
            Assert.Contains(cs.DailyComplaintId, leadersDailyComplaints.Select(d => d.DailyComplaintId));
            Assert.Equal((int)Status.Unresolved, cs.Status);
        });
        _mockDailyComplaintRepository.Verify(r => r.GetDailyComplaintsForLeaderByLeaderIdAsync(leaderId), Times.Once);
    }

    [Fact]
    public async Task GetUnresolvedComplaintsOfLeader_WhenNoMatches_ReturnsEmpty()
    {
        // Arrange
        var leaderId = 77;
        _mockDailyComplaintRepository
            .Setup(r => r.GetDailyComplaintsForLeaderByLeaderIdAsync(leaderId))
            .ReturnsAsync(new List<DailyComplaint>
            {
                new DailyComplaint { DailyComplaintId = 9, LeaderId = leaderId, ConstituencyId = 1, IsCompleted = false, DateCreated = DateTime.UtcNow, DateUpdated = DateTime.UtcNow }
            });

        var s = BuildDailyComplaintStatus(9, statusValue: (int)Status.InProgress, created: DateTime.UtcNow.AddMinutes(-2), updated: DateTime.UtcNow.AddMinutes(-1));
        _context.DailyComplaintStatuses.Add(s);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUnresolvedComplaintsOfLeader(leaderId, Status.Unresolved);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockDailyComplaintRepository.Verify(r => r.GetDailyComplaintsForLeaderByLeaderIdAsync(leaderId), Times.Once);
    }

    private DailyComplaintStatus BuildDailyComplaintStatus(int dailyComplaintId, int statusValue, DateTime created, DateTime updated)
    {
        return _fixture.Build<DailyComplaintStatus>()
            .With(s => s.DailyComplaintStatusId, 0)
            .With(s => s.DailyComplaintId, dailyComplaintId)
            .With(s => s.Status, statusValue)
            .With(s => s.DateCreated, created)
            .With(s => s.DateUpdated, updated)
            .Create();
    }
}
