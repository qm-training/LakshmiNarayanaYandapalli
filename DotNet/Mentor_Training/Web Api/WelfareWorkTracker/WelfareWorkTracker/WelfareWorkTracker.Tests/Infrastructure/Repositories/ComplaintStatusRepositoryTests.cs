namespace WelfareWorkTracker.Tests.Infrastructure.Repositories;
public class ComplaintStatusRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly WelfareWorkTrackerContext _context;
    private readonly Mock<IComplaintRepository> _mockComplaintRepository = new(MockBehavior.Strict);
    private readonly ComplaintStatusRepository _repository;

    public ComplaintStatusRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _repository = new ComplaintStatusRepository(_context, _mockComplaintRepository.Object);
    }

    // AddComplaintStatusAsync

    [Fact]
    public async Task AddComplaintStatusAsync_WithValidStatus_ReturnsPersistedEntity()
    {
        // Arrange
        var status = BuildStatus(complaintId: 10, statusValue: 2, updated: DateTime.UtcNow);

        // Act
        var result = await _repository.AddComplaintStatusAsync(status);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ComplaintStatusId > 0);
        var fetched = await _context.ComplaintStatuses.FindAsync(result.ComplaintStatusId);
        Assert.NotNull(fetched);
    }

    [Fact]
    public async Task AddComplaintStatusAsync_WithMinimalFields_PersistsAndSetsKey()
    {
        // Arrange
        var status = new ComplaintStatus
        {
            ComplaintStatusId = 0,
            ComplaintId = 5,
            Status = 1,
            DateUpdated = DateTime.UtcNow,
            OpenedDate = DateTime.UtcNow.AddHours(-1)
        };

        // Act
        var result = await _repository.AddComplaintStatusAsync(status);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ComplaintStatusId > 0);
        Assert.Equal(5, result.ComplaintId);
    }

    // GetComplaintStatusAsync

    [Fact]
    public async Task GetComplaintStatusAsync_WithComplaintId_ReturnsLatestByDateUpdated()
    {
        // Arrange
        var id = 77;
        var older = BuildStatus(id, statusValue: 1, updated: DateTime.UtcNow.AddMinutes(-5));
        var newer = BuildStatus(id, statusValue: 2, updated: DateTime.UtcNow.AddMinutes(1));
        _context.ComplaintStatuses.AddRange(older, newer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintStatusAsync(complaintId: id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newer.ComplaintStatusId, result!.ComplaintStatusId);
    }

    [Fact]
    public async Task GetComplaintStatusAsync_WithComplaintStatusId_ReturnsExactMatch()
    {
        // Arrange
        var s1 = BuildStatus(1, 1, DateTime.UtcNow.AddMinutes(-2));
        var s2 = BuildStatus(1, 2, DateTime.UtcNow.AddMinutes(-1));
        _context.ComplaintStatuses.AddRange(s1, s2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintStatusAsync(null, s1.ComplaintStatusId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(s1.ComplaintStatusId, result!.ComplaintStatusId);
    }

    [Fact]
    public async Task GetComplaintStatusAsync_WithoutIds_ThrowsArgumentException()
    {
        // Arrange

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetComplaintStatusAsync());
    }

    // UpdateComplaintStatus

    [Fact]
    public async Task UpdateComplaintStatus_WithExistingEntity_PersistsChanges()
    {
        // Arrange
        var s = BuildStatus(complaintId: 12, statusValue: 1, updated: DateTime.UtcNow.AddMinutes(-10));
        _context.ComplaintStatuses.Add(s);
        await _context.SaveChangesAsync();
        s.Status = 3;
        s.DateUpdated = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateComplaintStatus(s);

        // Assert
        Assert.NotNull(result);
        var fetched = await _context.ComplaintStatuses.FindAsync(s.ComplaintStatusId);
        Assert.Equal(3, fetched!.Status);
    }

    // DeleteComplaintStatusByComplaintIdAsync

    [Fact]
    public async Task DeleteComplaintStatusByComplaintIdAsync_WhenStatusWithCode1Exists_ReturnsTrueAndRemovesOne()
    {
        // Arrange
        var targetComplaintId = 100;
        var notTarget = BuildStatus(complaintId: 99, statusValue: 1, updated: DateTime.UtcNow);
        var targetNonDeletable = BuildStatus(complaintId: targetComplaintId, statusValue: 2, updated: DateTime.UtcNow);
        var targetDeletable = BuildStatus(complaintId: targetComplaintId, statusValue: 1, updated: DateTime.UtcNow.AddMinutes(-1));
        _context.ComplaintStatuses.AddRange(notTarget, targetNonDeletable, targetDeletable);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteComplaintStatusByComplaintIdAsync(targetComplaintId);

        // Assert
        Assert.True(result);
        var remaining = await _context.ComplaintStatuses.Where(cs => cs.ComplaintId == targetComplaintId).ToListAsync();
        Assert.DoesNotContain(remaining, cs => cs.Status == 1);
    }

    [Fact]
    public async Task DeleteComplaintStatusByComplaintIdAsync_WhenNoneWithCode1_ReturnsFalse()
    {
        // Arrange
        var targetComplaintId = 200;
        var a = BuildStatus(targetComplaintId, statusValue: 2, updated: DateTime.UtcNow);
        _context.ComplaintStatuses.Add(a);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteComplaintStatusByComplaintIdAsync(targetComplaintId);

        // Assert
        Assert.False(result);
        var stillThere = await _context.ComplaintStatuses.FindAsync(a.ComplaintStatusId);
        Assert.NotNull(stillThere);
    }

    // GetUnresolvedComplaintsOfLeader

    [Fact]
    public async Task GetUnresolvedComplaintsOfLeader_WithMatches_ReturnsStatusesForLeaderAndStatus()
    {
        // Arrange
        var leaderId = 5;
        var leaderComplaints = new List<Complaint>
        {
            new Complaint { ComplaintId = 1, LeaderId = leaderId },
            new Complaint { ComplaintId = 2, LeaderId = leaderId }
        };
        _mockComplaintRepository
            .Setup(r => r.GetComplaintsForLeaderByLeaderIdAsync(leaderId))
            .ReturnsAsync(leaderComplaints);

        var s1 = BuildStatus(complaintId: 1, statusValue: (int)Status.Unresolved, updated: DateTime.UtcNow);
        var s2 = BuildStatus(complaintId: 2, statusValue: (int)Status.Unresolved, updated: DateTime.UtcNow);
        var s3 = BuildStatus(complaintId: 3, statusValue: (int)Status.Unresolved, updated: DateTime.UtcNow);
        var s4 = BuildStatus(complaintId: 2, statusValue: (int)Status.Resolved, updated: DateTime.UtcNow);
        _context.ComplaintStatuses.AddRange(s1, s2, s3, s4);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUnresolvedComplaintsOfLeader(leaderId, Status.Unresolved);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, cs =>
        {
            Assert.Contains(cs.ComplaintId, leaderComplaints.Select(c => c.ComplaintId));
            Assert.Equal((int)Status.Unresolved, cs.Status);
        });
        _mockComplaintRepository.Verify(r => r.GetComplaintsForLeaderByLeaderIdAsync(leaderId), Times.Once);
    }

    [Fact]
    public async Task GetUnresolvedComplaintsOfLeader_WhenNoMatches_ReturnsEmpty()
    {
        // Arrange
        var leaderId = 8;
        _mockComplaintRepository
            .Setup(r => r.GetComplaintsForLeaderByLeaderIdAsync(leaderId))
            .ReturnsAsync(new List<Complaint> { new Complaint { ComplaintId = 11, LeaderId = leaderId } });

        var s = BuildStatus(complaintId: 11, statusValue: (int)Status.InProgress, updated: DateTime.UtcNow);
        _context.ComplaintStatuses.Add(s);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUnresolvedComplaintsOfLeader(leaderId, Status.Unresolved);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockComplaintRepository.Verify(r => r.GetComplaintsForLeaderByLeaderIdAsync(leaderId), Times.Once);
    }

    // GetComplaintStatusHistoryByComplaintIdAsync

    [Fact]
    public async Task GetComplaintStatusHistoryByComplaintIdAsync_WithStatuses_ReturnsOrderedByDateUpdatedAscending()
    {
        // Arrange
        var cId = 300;
        var s1 = BuildStatus(cId, 1, DateTime.UtcNow.AddMinutes(-10));
        var s2 = BuildStatus(cId, 2, DateTime.UtcNow.AddMinutes(-5));
        var s3 = BuildStatus(cId, 3, DateTime.UtcNow.AddMinutes(-1));
        var other = BuildStatus(301, 1, DateTime.UtcNow);
        _context.ComplaintStatuses.AddRange(s1, s2, s3, other);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintStatusHistoryByComplaintIdAsync(cId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        var dates = result.Select(r => r.DateUpdated).ToList();
        var sorted = dates.OrderBy(d => d).ToList();
        Assert.True(dates.SequenceEqual(sorted));
        Assert.All(result, r => Assert.Equal(cId, r.ComplaintId));
    }

    [Fact]
    public async Task GetComplaintStatusHistoryByComplaintIdAsync_WhenNone_ReturnsEmpty()
    {
        // Arrange

        // Act
        var result = await _repository.GetComplaintStatusHistoryByComplaintIdAsync(9999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    private ComplaintStatus BuildStatus(int complaintId, int statusValue, DateTime updated)
    {
        return _fixture.Build<ComplaintStatus>()
            .With(s => s.ComplaintStatusId, 0)
            .With(s => s.ComplaintId, complaintId)
            .With(s => s.Status, statusValue)
            .With(s => s.OpenedDate, updated.AddHours(-1))
            .With(s => s.DateUpdated, updated)
            .Create();
    }
}
