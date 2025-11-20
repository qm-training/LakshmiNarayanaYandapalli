namespace WelfareWorkTracker.Tests.Infrastructure.Repositories;
public class DailyComplaintRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly WelfareWorkTrackerContext _context;
    private readonly DailyComplaintRepository _repository;

    public DailyComplaintRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _repository = new DailyComplaintRepository(_context);
    }

    // AddDailyComplaintAsync

    [Fact]
    public async Task AddDailyComplaintAsync_WithValidEntity_ReturnsPersistedEntity()
    {
        // Arrange
        var entity = BuildDailyComplaint(leaderId: 10, constituencyId: 1, when: DateTime.UtcNow, isCompleted: false);

        // Act
        var result = await _repository.AddDailyComplaintAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.DailyComplaintId > 0);
        var fetched = await _context.DailyComplaints.FindAsync(result.DailyComplaintId);
        Assert.NotNull(fetched);
        Assert.Equal(entity.LeaderId, fetched!.LeaderId);
        Assert.Equal(entity.ConstituencyId, fetched.ConstituencyId);
        Assert.Equal(entity.IsCompleted, fetched.IsCompleted);
    }

    [Fact]
    public async Task AddDailyComplaintAsync_WithMinimalValidFields_PersistsAndSetsKey()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entity = new DailyComplaint
        {
            DailyComplaintId = 0,
            LeaderId = 5,
            ConstituencyId = 2,
            IsCompleted = false,
            DateCreated = now,
            DateUpdated = now
        };

        // Act
        var result = await _repository.AddDailyComplaintAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.DailyComplaintId > 0);
        Assert.Equal(5, result.LeaderId);
        Assert.Equal(2, result.ConstituencyId);
    }

    // GetDailyComplaintsAsync

    [Fact]
    public async Task GetDailyComplaintsAsync_WithExistingData_ReturnsAll()
    {
        // Arrange
        var a = BuildDailyComplaint(1, 11, DateTime.UtcNow.AddMinutes(-2), false);
        var b = BuildDailyComplaint(2, 12, DateTime.UtcNow.AddMinutes(-1), true);
        _context.DailyComplaints.AddRange(a, b);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDailyComplaintsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count >= 2);
        Assert.Contains(result, d => d.DailyComplaintId == a.DailyComplaintId);
        Assert.Contains(result, d => d.DailyComplaintId == b.DailyComplaintId);
    }

    [Fact]
    public async Task GetDailyComplaintsAsync_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange

        // Act
        var result = await _repository.GetDailyComplaintsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // GetDailyComplaintByIdAsync

    [Fact]
    public async Task GetDailyComplaintByIdAsync_WithExistingId_ReturnsEntity()
    {
        // Arrange
        var dc = BuildDailyComplaint(leaderId: 3, constituencyId: 21, when: DateTime.UtcNow, isCompleted: false);
        _context.DailyComplaints.Add(dc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDailyComplaintByIdAsync(dc.DailyComplaintId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dc.DailyComplaintId, result!.DailyComplaintId);
    }

    [Fact]
    public async Task GetDailyComplaintByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var dc = BuildDailyComplaint(leaderId: 4, constituencyId: 22, when: DateTime.UtcNow, isCompleted: true);
        _context.DailyComplaints.Add(dc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDailyComplaintByIdAsync(dc.DailyComplaintId + 1000);

        // Assert
        Assert.Null(result);
    }

    // UpdateDailyComplaintAsync

    [Fact]
    public async Task UpdateDailyComplaintAsync_WithTrackedEntity_PersistsChanges()
    {
        // Arrange
        var dc = BuildDailyComplaint(leaderId: 5, constituencyId: 30, when: DateTime.UtcNow.AddMinutes(-10), isCompleted: false);
        _context.DailyComplaints.Add(dc);
        await _context.SaveChangesAsync();
        dc.IsCompleted = true;
        dc.DateUpdated = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateDailyComplaintAsync(dc);

        // Assert
        Assert.NotNull(result);
        var fetched = await _context.DailyComplaints.FindAsync(dc.DailyComplaintId);
        Assert.True(fetched!.IsCompleted);
        Assert.True(fetched.DateUpdated >= dc.DateUpdated);
    }

    [Fact]
    public async Task UpdateDailyComplaintAsync_WithDetachedEntity_PersistsChanges()
    {
        // Arrange
        var existing = BuildDailyComplaint(leaderId: 6, constituencyId: 31, when: DateTime.UtcNow.AddMinutes(-20), isCompleted: false);
        _context.DailyComplaints.Add(existing);
        await _context.SaveChangesAsync();
        _context.Entry(existing).State = EntityState.Detached;

        var detached = new DailyComplaint
        {
            DailyComplaintId = existing.DailyComplaintId,
            LeaderId = existing.LeaderId,
            ConstituencyId = existing.ConstituencyId,
            IsCompleted = true,
            DateCreated = existing.DateCreated,
            DateUpdated = DateTime.UtcNow
        };

        // Act
        var result = await _repository.UpdateDailyComplaintAsync(detached);

        // Assert
        Assert.NotNull(result);
        var fetched = await _context.DailyComplaints.FindAsync(existing.DailyComplaintId);
        Assert.True(fetched!.IsCompleted);
        Assert.Equal(detached.LeaderId, fetched.LeaderId);
        Assert.Equal(detached.ConstituencyId, fetched.ConstituencyId);
    }

    // GetDailyComplaintByLeaderIdAsync

    [Fact]
    public async Task GetDailyComplaintByLeaderIdAsync_WithTodayRecord_ReturnsEntity()
    {
        // Arrange
        var leaderId = 77;
        var todayLocal = DateTime.Today.AddHours(10);
        var match = BuildDailyComplaint(leaderId, constituencyId: 40, when: todayLocal, isCompleted: false);
        var otherDay = BuildDailyComplaint(leaderId, constituencyId: 40, when: todayLocal.AddDays(-1), isCompleted: false);
        _context.DailyComplaints.AddRange(match, otherDay);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDailyComplaintByLeaderIdAsync(leaderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(match.DailyComplaintId, result!.DailyComplaintId);
    }

    [Fact]
    public async Task GetDailyComplaintByLeaderIdAsync_WithNoTodayRecord_ReturnsNull()
    {
        // Arrange
        var leaderId = 78;
        var notToday = BuildDailyComplaint(leaderId, constituencyId: 41, when: DateTime.Today.AddDays(-1), isCompleted: false);
        _context.DailyComplaints.Add(notToday);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDailyComplaintByLeaderIdAsync(leaderId);

        // Assert
        Assert.Null(result);
    }

    // GetTodaysDailyComplaintsAsync (implementation actually returns previous day's by UTC date)

    [Fact]
    public async Task GetTodaysDailyComplaintsAsync_WithPreviousUtcDayData_ReturnsMatches()
    {
        // Arrange
        var previousDayUtc = DateTime.UtcNow.Date.AddDays(-1).AddHours(9);
        var match1 = BuildDailyComplaint(leaderId: 90, constituencyId: 50, when: previousDayUtc, isCompleted: false);
        var match2 = BuildDailyComplaint(leaderId: 91, constituencyId: 51, when: previousDayUtc.AddMinutes(30), isCompleted: true);
        var todayUtc = DateTime.UtcNow.Date.AddHours(8);
        var notMatch = BuildDailyComplaint(leaderId: 92, constituencyId: 52, when: todayUtc, isCompleted: false);
        _context.DailyComplaints.AddRange(match1, match2, notMatch);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTodaysDailyComplaintsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, d => Assert.Equal(previousDayUtc.Date, d.DateCreated.Date));
    }

    [Fact]
    public async Task GetTodaysDailyComplaintsAsync_WithNoPreviousUtcDayData_ReturnsEmpty()
    {
        // Arrange
        var todayOnly = BuildDailyComplaint(leaderId: 93, constituencyId: 53, when: DateTime.UtcNow.Date.AddHours(5), isCompleted: false);
        _context.DailyComplaints.Add(todayOnly);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTodaysDailyComplaintsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // GetDailyComplaintsForLeaderByLeaderIdAsync

    [Fact]
    public async Task GetDailyComplaintsForLeaderByLeaderIdAsync_WithLeader_ReturnsOnlyLeadersComplaints()
    {
        // Arrange
        var mine1 = BuildDailyComplaint(leaderId: 101, constituencyId: 60, when: DateTime.UtcNow, isCompleted: false);
        var mine2 = BuildDailyComplaint(leaderId: 101, constituencyId: 60, when: DateTime.UtcNow.AddMinutes(1), isCompleted: true);
        var other = BuildDailyComplaint(leaderId: 102, constituencyId: 61, when: DateTime.UtcNow, isCompleted: false);
        _context.DailyComplaints.AddRange(mine1, mine2, other);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDailyComplaintsForLeaderByLeaderIdAsync(101);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, d => Assert.Equal(101, d.LeaderId));
    }

    [Fact]
    public async Task GetDailyComplaintsForLeaderByLeaderIdAsync_WithNoMatches_ReturnsEmpty()
    {
        // Arrange
        var other = BuildDailyComplaint(leaderId: 202, constituencyId: 62, when: DateTime.UtcNow, isCompleted: false);
        _context.DailyComplaints.Add(other);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDailyComplaintsForLeaderByLeaderIdAsync(201);

        // Assert
        Assert.Empty(result);
    }

    // GetDailyComplaintsByConstituencyNameAsync (join with Constituencies + DateCreated.Date == Today)

    [Fact]
    public async Task GetDailyComplaintsByConstituencyNameAsync_WithTodayAndMatchingName_ReturnsEntity()
    {
        // Arrange
        var consty = BuildConstituency("Central");
        _context.Constituencies.Add(consty);
        await _context.SaveChangesAsync();

        var todayLocal = DateTime.Today.AddHours(9);
        var match = BuildDailyComplaint(leaderId: 303, constituencyId: consty.ConstituencyId, when: todayLocal, isCompleted: false);
        var wrongDate = BuildDailyComplaint(leaderId: 303, constituencyId: consty.ConstituencyId, when: todayLocal.AddDays(-1), isCompleted: false);
        _context.DailyComplaints.AddRange(match, wrongDate);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDailyComplaintsByConstituencyNameAsync("Central");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(match.DailyComplaintId, result!.DailyComplaintId);
    }

    [Fact]
    public async Task GetDailyComplaintsByConstituencyNameAsync_WithNameMismatchOrNotToday_ReturnsNull()
    {
        // Arrange
        var c1 = BuildConstituency("Harbor");
        var c2 = BuildConstituency("Uptown");
        _context.Constituencies.AddRange(c1, c2);
        await _context.SaveChangesAsync();

        var yesterdayLocal = DateTime.Today.AddDays(-1).AddHours(10);
        var dc = BuildDailyComplaint(leaderId: 404, constituencyId: c1.ConstituencyId, when: yesterdayLocal, isCompleted: false);
        _context.DailyComplaints.Add(dc);
        await _context.SaveChangesAsync();

        // Act
        var missByName = await _repository.GetDailyComplaintsByConstituencyNameAsync("Uptown");
        var missByDate = await _repository.GetDailyComplaintsByConstituencyNameAsync("Harbor");

        // Assert
        Assert.Null(missByName);
        Assert.Null(missByDate);
    }

    private DailyComplaint BuildDailyComplaint(int leaderId, int constituencyId, DateTime when, bool isCompleted)
    {
        return _fixture.Build<DailyComplaint>()
            .With(d => d.DailyComplaintId, 0)
            .With(d => d.LeaderId, leaderId)
            .With(d => d.ConstituencyId, constituencyId)
            .With(d => d.IsCompleted, isCompleted)
            .With(d => d.DateCreated, when)
            .With(d => d.DateUpdated, when)
            .Create();
    }

    private Constituency BuildConstituency(string name)
    {
        return _fixture.Build<Constituency>()
            .With(c => c.ConstituencyId, 0)
            .With(c => c.ConstituencyName, name)
            .With(c => c.DistrictName, "District")
            .With(c => c.StateName, "State")
            .With(c => c.CountryName, "Country")
            .Create();
    }
}
