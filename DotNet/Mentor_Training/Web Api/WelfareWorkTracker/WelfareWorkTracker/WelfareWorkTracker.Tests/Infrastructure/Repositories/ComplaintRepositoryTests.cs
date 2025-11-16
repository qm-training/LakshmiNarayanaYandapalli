namespace WelfareWorkTracker.Tests.Infrastructure.Repositories;
public class ComplaintRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly WelfareWorkTrackerContext _context;
    private readonly ComplaintRepository _repository;

    public ComplaintRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _repository = new ComplaintRepository(_context);
    }

    // AddComplaintAsync

    [Fact]
    public async Task AddComplaintAsync_WithValidComplaint_ReturnsPersistedComplaint()
    {
        // Arrange
        var complaint = BuildComplaint(citizenId: 10, constituency: "Alpha");

        // Act
        var result = await _repository.AddComplaintAsync(complaint);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ComplaintId > 0);
        var fetched = await _context.Complaints.FindAsync(result.ComplaintId);
        Assert.NotNull(fetched);
        Assert.Equal("Alpha", fetched!.ConstituencyName);
    }

    [Fact]
    public async Task AddComplaintAsync_WithMinimalValidFields_PersistsAndSetsKey()
    {
        // Arrange
        var complaint = new Complaint
        {
            ComplaintId = 0,
            Title = "Street Light",
            Description = "Not working",
            ConstituencyName = "Beta",
            CitizenId = 99,
            DateCreated = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddComplaintAsync(complaint);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ComplaintId > 0);
        Assert.Equal("Beta", result.ConstituencyName);
    }

    // DeleteComplaintByComplaintAsync

    [Fact]
    public async Task DeleteComplaintByComplaintAsync_WithTrackedEntity_RemovesComplaint()
    {
        // Arrange
        var existing = BuildComplaint(1, "Gamma");
        _context.Complaints.Add(existing);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteComplaintByComplaintAsync(existing);

        // Assert
        var gone = await _context.Complaints.FindAsync(existing.ComplaintId);
        Assert.Null(gone);
    }

    [Fact]
    public async Task DeleteComplaintByComplaintAsync_WithDetachedEntity_RemovesComplaint()
    {
        // Arrange
        var existing = BuildComplaint(2, "Delta");
        _context.Complaints.Add(existing);
        await _context.SaveChangesAsync();
        _context.Entry(existing).State = EntityState.Detached;
        var toRemove = new Complaint { ComplaintId = existing.ComplaintId };

        // Act
        await _repository.DeleteComplaintByComplaintAsync(toRemove);

        // Assert
        var gone = await _context.Complaints.FindAsync(existing.ComplaintId);
        Assert.Null(gone);
    }

    // GetComplaintByComplaintIdAsync

    [Fact]
    public async Task GetComplaintByComplaintIdAsync_WithExistingId_ReturnsComplaint()
    {
        // Arrange
        var existing = BuildComplaint(3, "Eta");
        _context.Complaints.Add(existing);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintByComplaintIdAsync(existing.ComplaintId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existing.ComplaintId, result!.ComplaintId);
    }

    [Fact]
    public async Task GetComplaintByComplaintIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var existing = BuildComplaint(4, "Zeta");
        _context.Complaints.Add(existing);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintByComplaintIdAsync(existing.ComplaintId + 1000);

        // Assert
        Assert.Null(result);
    }

    // GetComplaintsAsync

    [Fact]
    public async Task GetComplaintsAsync_WithExistingData_ReturnsAllComplaints()
    {
        // Arrange
        var a = BuildComplaint(11, "A");
        var b = BuildComplaint(12, "B");
        _context.Complaints.AddRange(a, b);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count >= 2);
        Assert.Contains(result, c => c.ComplaintId == a.ComplaintId);
        Assert.Contains(result, c => c.ComplaintId == b.ComplaintId);
    }

    [Fact]
    public async Task GetComplaintsAsync_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange

        // Act
        var result = await _repository.GetComplaintsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // GetComplaintsByComplaintIdsAsync

    [Fact]
    public async Task GetComplaintsByComplaintIdsAsync_WithIds_ReturnsMatchingComplaints()
    {
        // Arrange
        var c1 = BuildComplaint(21, "C1");
        var c2 = BuildComplaint(22, "C2");
        var c3 = BuildComplaint(23, "C3");
        _context.Complaints.AddRange(c1, c2, c3);
        await _context.SaveChangesAsync();
        var ids = new List<int> { c1.ComplaintId, c3.ComplaintId };

        // Act
        var result = await _repository.GetComplaintsByComplaintIdsAsync(ids);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.ComplaintId == c1.ComplaintId);
        Assert.Contains(result, c => c.ComplaintId == c3.ComplaintId);
        Assert.DoesNotContain(result, c => c.ComplaintId == c2.ComplaintId);
    }

    [Fact]
    public async Task GetComplaintsByComplaintIdsAsync_WithNoMatchingIds_ReturnsEmpty()
    {
        // Arrange
        var c1 = BuildComplaint(24, "C4");
        _context.Complaints.Add(c1);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintsByComplaintIdsAsync(new List<int> { c1.ComplaintId + 100 });

        // Assert
        Assert.Empty(result);
    }

    // GetComplaintsByConstituency

    [Fact]
    public async Task GetComplaintsByConstituency_WithName_ReturnsOnlyThatConstituency()
    {
        // Arrange
        var x1 = BuildComplaint(31, "Hyd"); x1.ConstituencyName = "Central";
        var x2 = BuildComplaint(32, "Hyd"); x2.ConstituencyName = "Central";
        var y1 = BuildComplaint(33, "Vizag"); y1.ConstituencyName = "Harbor";
        _context.Complaints.AddRange(x1, x2, y1);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintsByConstituency("Central");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Equal("Central", c.ConstituencyName));
    }

    [Fact]
    public async Task GetComplaintsByConstituency_WithNoMatches_ReturnsEmpty()
    {
        // Arrange
        var x1 = BuildComplaint(34, "Hyd"); x1.ConstituencyName = "North";
        _context.Complaints.Add(x1);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintsByConstituency("South");

        // Assert
        Assert.Empty(result);
    }

    // GetComplaintsByUserIdAsync

    [Fact]
    public async Task GetComplaintsByUserIdAsync_WithUser_ReturnsOnlyUserComplaints()
    {
        // Arrange
        var mine1 = BuildComplaint(100, "X"); mine1.CitizenId = 7;
        var mine2 = BuildComplaint(100, "Y"); mine2.CitizenId = 7;
        var other = BuildComplaint(200, "Z"); other.CitizenId = 8;
        _context.Complaints.AddRange(mine1, mine2, other);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintsByUserIdAsync(7);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Equal(7, c.CitizenId));
    }

    [Fact]
    public async Task GetComplaintsByUserIdAsync_WithNoComplaints_ReturnsEmpty()
    {
        // Arrange
        var other = BuildComplaint(201, "Q"); other.CitizenId = 9;
        _context.Complaints.Add(other);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintsByUserIdAsync(7);

        // Assert
        Assert.Empty(result);
    }

    // GetComplaintsForLeaderByLeaderIdAsync

    [Fact]
    public async Task GetComplaintsForLeaderByLeaderIdAsync_WithLeader_ReturnsOnlyLeaderComplaints()
    {
        // Arrange
        var l1 = BuildComplaint(300, "L"); l1.LeaderId = 55;
        var l2 = BuildComplaint(301, "L"); l2.LeaderId = 55;
        var other = BuildComplaint(302, "L"); other.LeaderId = 77;
        _context.Complaints.AddRange(l1, l2, other);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintsForLeaderByLeaderIdAsync(55);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Equal(55, c.LeaderId));
    }

    [Fact]
    public async Task GetComplaintsForLeaderByLeaderIdAsync_WithNoMatches_ReturnsEmpty()
    {
        // Arrange
        var other = BuildComplaint(303, "L"); other.LeaderId = 77;
        _context.Complaints.Add(other);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintsForLeaderByLeaderIdAsync(55);

        // Assert
        Assert.Empty(result);
    }

    // GetComplaintsInBacklogForMoreThanHoursAsync

    [Fact]
    public async Task GetComplaintsInBacklogForMoreThanHoursAsync_WithOlderBacklog_ReturnsOnlyThose()
    {
        // Arrange
        var now = DateTime.Now;
        var s1_old_backlog = BuildStatus(complaintId: 1, status: (int)Status.Backlog, opened: now.AddHours(-10), updated: now.AddHours(-9));
        var s1_new_progress = BuildStatus(complaintId: 1, status: (int)Status.InProgress, opened: now.AddHours(-2), updated: now.AddHours(-1));

        var s2_old_backlog = BuildStatus(complaintId: 2, status: (int)Status.Backlog, opened: now.AddHours(-8), updated: now.AddHours(-8.5));
        var s2_new_backlog = BuildStatus(complaintId: 2, status: (int)Status.Backlog, opened: now.AddHours(-7), updated: now.AddHours(-6)); // latest stays backlog

        var s3_recent_backlog = BuildStatus(complaintId: 3, status: (int)Status.Backlog, opened: now.AddHours(-1), updated: now.AddMinutes(-30));

        _context.ComplaintStatuses.AddRange(s1_old_backlog, s1_new_progress, s2_old_backlog, s2_new_backlog, s3_recent_backlog);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintsInBacklogForMoreThanHoursAsync(5);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, cs => Assert.Equal((int)Status.Backlog, cs!.Status));
        Assert.DoesNotContain(result, cs => cs!.ComplaintId == 1); // latest for 1 is InProgress
        Assert.Contains(result, cs => cs!.ComplaintId == 2);       // latest for 2 is Backlog and older than cutoff
        Assert.DoesNotContain(result, cs => cs!.ComplaintId == 3); // recent, not older than cutoff
    }

    [Fact]
    public async Task GetComplaintsInBacklogForMoreThanHoursAsync_WithNoMatches_ReturnsEmpty()
    {
        // Arrange
        var now = DateTime.Now;
        var s1 = BuildStatus(complaintId: 10, status: (int)Status.InProgress, opened: now.AddHours(-10), updated: now.AddHours(-1));
        var s2 = BuildStatus(complaintId: 11, status: (int)Status.Resolved, opened: now.AddHours(-10), updated: now.AddHours(-1));
        _context.ComplaintStatuses.AddRange(s1, s2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintsInBacklogForMoreThanHoursAsync(5);

        // Assert
        Assert.Empty(result);
    }

    // GetRecentComplaintsAsync

    [Fact]
    public async Task GetRecentComplaintsAsync_WithMoreThanTen_ReturnsTop10OrderedByDate()
    {
        // Arrange
        var list = new List<Complaint>();
        for (int i = 0; i < 12; i++)
        {
            var c = BuildComplaint(400 + i, "Metro");
            c.ConstituencyName = "Central";
            c.DateCreated = DateTime.UtcNow.AddMinutes(i);
            list.Add(c);
        }
        _context.Complaints.AddRange(list);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRecentComplaintsAsync("Central");

        // Assert
        Assert.Equal(10, result.Count);
        var dates = result.Select(r => r.DateCreated).ToList();
        var sorted = dates.OrderByDescending(d => d).ToList();
        Assert.True(dates.SequenceEqual(sorted));
        Assert.All(result, r => Assert.Equal("Central", r.ConstituencyName));
    }

    [Fact]
    public async Task GetRecentComplaintsAsync_WithFewerThanTen_ReturnsAllOrderedByDate()
    {
        // Arrange
        var a = BuildComplaint(501, "A"); a.ConstituencyName = "West"; a.DateCreated = DateTime.UtcNow.AddMinutes(1);
        var b = BuildComplaint(502, "B"); b.ConstituencyName = "West"; b.DateCreated = DateTime.UtcNow.AddMinutes(3);
        var c = BuildComplaint(503, "C"); c.ConstituencyName = "East"; c.DateCreated = DateTime.UtcNow.AddMinutes(2);
        _context.Complaints.AddRange(a, b, c);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRecentComplaintsAsync("West");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(b.ComplaintId, result[0].ComplaintId);
        Assert.Equal(a.ComplaintId, result[1].ComplaintId);
    }

    // UpdateComplaintByIdAsync

    [Fact]
    public async Task UpdateComplaintByIdAsync_WithExistingComplaint_PersistsChanges()
    {
        // Arrange
        var existing = BuildComplaint(600, "Pre");
        _context.Complaints.Add(existing);
        await _context.SaveChangesAsync();
        existing.Title = "Updated Title";
        existing.Description = "Updated Description";
        existing.DateUpdated = DateTime.UtcNow;

        // Act
        await _repository.UpdateComplaintByIdAsync(existing);

        // Assert
        var fetched = await _context.Complaints.FindAsync(existing.ComplaintId);
        Assert.NotNull(fetched);
        Assert.Equal("Updated Title", fetched!.Title);
        Assert.Equal("Updated Description", fetched.Description);
    }

    [Fact]
    public async Task UpdateComplaintByIdAsync_WithDetachedEntity_PersistsChanges()
    {
        // Arrange
        var existing = BuildComplaint(601, "Pre2");
        _context.Complaints.Add(existing);
        await _context.SaveChangesAsync();
        _context.Entry(existing).State = EntityState.Detached;

        var detached = new Complaint
        {
            ComplaintId = existing.ComplaintId,
            Title = "DetUpd",
            Description = "DetDesc",
            ConstituencyName = existing.ConstituencyName,
            CitizenId = existing.CitizenId,
            LeaderId = existing.LeaderId,
            DateCreated = existing.DateCreated,
            DateUpdated = DateTime.UtcNow
        };

        // Act
        await _repository.UpdateComplaintByIdAsync(detached);

        // Assert
        var fetched = await _context.Complaints.FindAsync(existing.ComplaintId);
        Assert.NotNull(fetched);
        Assert.Equal("DetUpd", fetched!.Title);
        Assert.Equal("DetDesc", fetched.Description);
    }

    private Complaint BuildComplaint(int citizenId, string constituency)
    {
        return _fixture.Build<Complaint>()
            .With(c => c.ComplaintId, 0)
            .With(c => c.CitizenId, citizenId)
            .With(c => c.ConstituencyName, constituency)
            .With(c => c.Title, $"Title-{Guid.NewGuid():N}")
            .With(c => c.Description, $"Desc-{Guid.NewGuid():N}")
            .With(c => c.DateCreated, DateTime.UtcNow)
            .Create();
    }

    private ComplaintStatus BuildStatus(int complaintId, int status, DateTime opened, DateTime updated)
    {
        return _fixture.Build<ComplaintStatus>()
            .With(s => s.ComplaintStatusId, 0)
            .With(s => s.ComplaintId, complaintId)
            .With(s => s.Status, status)
            .With(s => s.OpenedDate, opened)
            .With(s => s.DateUpdated, updated)
            .Create();
    }
}
