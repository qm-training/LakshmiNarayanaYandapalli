namespace WelfareWorkTracker.Tests.Infrastructure.Services;
public class DailyComplaintServiceTests
{
    private readonly Mock<IDailyComplaintRepository> _mockDailyComplaintRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IDailyComplaintStatusRepository> _mockDailyComplaintStatusRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly DailyComplaintService _service;

    public DailyComplaintServiceTests()
    {
        _mockDailyComplaintRepo = new Mock<IDailyComplaintRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockDailyComplaintStatusRepo = new Mock<IDailyComplaintStatusRepository>();
        _mockMapper = new Mock<IMapper>();

        _service = new DailyComplaintService(
            _mockDailyComplaintRepo.Object,
            _mockUserRepo.Object,
            _mockDailyComplaintStatusRepo.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task GetDailyComplaintByIdAsync_WithExistingId_ReturnsDto()
    {
        // Arrange
        var entity = CreateDailyComplaint(10, 101, 201, false);
        var dto = CreateDailyComplaintDto(entity);
        _mockDailyComplaintRepo.Setup(r => r.GetDailyComplaintByIdAsync(10)).ReturnsAsync(entity);
        _mockMapper.Setup(m => m.Map<DailyComplaintDto>(entity)).Returns(dto);

        // Act
        var result = await _service.GetDailyComplaintByIdAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result!.DailyComplaintId);
        _mockDailyComplaintRepo.Verify(r => r.GetDailyComplaintByIdAsync(10), Times.Once);
        _mockMapper.Verify(m => m.Map<DailyComplaintDto>(entity), Times.Once);
    }

    [Fact]
    public async Task GetDailyComplaintByIdAsync_WithMissingId_ReturnsNull()
    {
        // Arrange
        _mockDailyComplaintRepo.Setup(r => r.GetDailyComplaintByIdAsync(99)).ReturnsAsync((DailyComplaint?)null);

        // Act
        var result = await _service.GetDailyComplaintByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetDailyComplaintByLeaderIdAsync_WithExistingLeader_ReturnsDto()
    {
        // Arrange
        var entity = CreateDailyComplaint(11, 102, 301, false);
        var dto = CreateDailyComplaintDto(entity);
        _mockDailyComplaintRepo.Setup(r => r.GetDailyComplaintByLeaderIdAsync(301)).ReturnsAsync(entity);
        _mockMapper.Setup(m => m.Map<DailyComplaintDto>(entity)).Returns(dto);

        // Act
        var result = await _service.GetDailyComplaintByLeaderIdAsync(301);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(11, result!.DailyComplaintId);
        _mockDailyComplaintRepo.Verify(r => r.GetDailyComplaintByLeaderIdAsync(301), Times.Once);
    }

    [Fact]
    public async Task GetDailyComplaintByLeaderIdAsync_WithNoAssignment_ReturnsNull()
    {
        // Arrange
        _mockDailyComplaintRepo.Setup(r => r.GetDailyComplaintByLeaderIdAsync(777)).ReturnsAsync((DailyComplaint?)null);

        // Act
        var result = await _service.GetDailyComplaintByLeaderIdAsync(777);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetDailyComplaintsAsync_WithData_ReturnsDtos()
    {
        // Arrange
        var entities = new List<DailyComplaint>
            {
                CreateDailyComplaint(1, 100, 200, false),
                CreateDailyComplaint(2, 101, 201, true)
            };
        var dtos = new List<DailyComplaintDto>
            {
                CreateDailyComplaintDto(entities[0]),
                CreateDailyComplaintDto(entities[1])
            };

        _mockDailyComplaintRepo.Setup(r => r.GetDailyComplaintsAsync()).ReturnsAsync(entities);
        _mockMapper.Setup(m => m.Map<List<DailyComplaintDto>>(entities)).Returns(dtos);

        // Act
        var result = await _service.GetDailyComplaintsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Collection(result,
            first => Assert.Equal(1, first.DailyComplaintId),
            second => Assert.Equal(2, second.DailyComplaintId));
        _mockDailyComplaintRepo.Verify(r => r.GetDailyComplaintsAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<List<DailyComplaintDto>>(entities), Times.Once);
    }

    [Fact]
    public async Task UpdateDailyComplaintAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockDailyComplaintRepo.Setup(r => r.GetDailyComplaintByIdAsync(999)).ReturnsAsync((DailyComplaint?)null);

        // Act
        var result = await _service.UpdateDailyComplaintAsync(999, new DailyComplaintVm());

        // Assert
        Assert.Null(result);
        _mockDailyComplaintRepo.Verify(r => r.UpdateDailyComplaintAsync(It.IsAny<DailyComplaint>()), Times.Never);
    }

    [Fact]
    public async Task AssignDailyComplaintsAsync_WithEligibleAndIneligibleLeaders_AssignsForEligibleOnly()
    {
        // Arrange
        var eligible = CreateUser(1, 10, "a@a.com", reputation: 20);
        var ineligibleEqual15 = CreateUser(2, 11, "b@b.com", reputation: 15);
        var ineligibleLow = CreateUser(3, 12, "c@c.com", reputation: 0);

        _mockUserRepo.Setup(r => r.GetAllLeadersAsync())
            .ReturnsAsync(new List<User> { eligible, ineligibleEqual15, ineligibleLow });

        var assignedIds = new List<int>();
        _mockDailyComplaintRepo
            .Setup(r => r.AddDailyComplaintAsync(It.IsAny<DailyComplaint>()))
            .Callback<DailyComplaint>(dc =>
            {
                dc.DailyComplaintId = 1000 + assignedIds.Count + 1;
                assignedIds.Add(dc.DailyComplaintId);
            })
            .Returns<DailyComplaint>(dc => Task.FromResult(dc));

        _mockDailyComplaintStatusRepo
            .Setup(r => r.AddDailyComplaintStatusAsync(It.IsAny<DailyComplaintStatus>()))
            .Returns<DailyComplaintStatus>(status => Task.FromResult(status));

        // Act
        await _service.AssignDailyComplaintsAsync();

        // Assert
        Assert.Single(assignedIds);

        _mockDailyComplaintRepo.Verify(r => r.AddDailyComplaintAsync(It.Is<DailyComplaint>(dc =>
            dc.LeaderId == eligible.UserId && dc.ConstituencyId == eligible.ConstituencyId && !dc.IsCompleted)), Times.Once);

        _mockDailyComplaintRepo.Verify(r => r.AddDailyComplaintAsync(It.Is<DailyComplaint>(dc =>
            dc.LeaderId == ineligibleEqual15.UserId)), Times.Never);

        _mockDailyComplaintRepo.Verify(r => r.AddDailyComplaintAsync(It.Is<DailyComplaint>(dc =>
            dc.LeaderId == ineligibleLow.UserId)), Times.Never);

        _mockDailyComplaintStatusRepo.Verify(r => r.AddDailyComplaintStatusAsync(It.Is<DailyComplaintStatus>(s =>
            s.DailyComplaintId == assignedIds[0])), Times.Once);
    }

    [Fact]
    public async Task GetDailyComplaintByConstituencyNameAsync_WithExisting_ReturnsDto()
    {
        // Arrange
        var entity = CreateDailyComplaint(77, 701, 801, false);
        var dto = CreateDailyComplaintDto(entity);
        _mockDailyComplaintRepo.Setup(r => r.GetDailyComplaintsByConstituencyNameAsync("North")).ReturnsAsync(entity);
        _mockMapper.Setup(m => m.Map<DailyComplaintDto>(entity)).Returns(dto);

        // Act
        var result = await _service.GetDailyComplaintByConstituencyNameAsync("North");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(77, result!.DailyComplaintId);
        _mockDailyComplaintRepo.Verify(r => r.GetDailyComplaintsByConstituencyNameAsync("North"), Times.Once);
        _mockMapper.Verify(m => m.Map<DailyComplaintDto>(entity), Times.Once);
    }

    [Fact]
    public async Task GetDailyComplaintByConstituencyNameAsync_WithMissing_ReturnsNull()
    {
        // Arrange
        _mockDailyComplaintRepo.Setup(r => r.GetDailyComplaintsByConstituencyNameAsync("Nowhere")).ReturnsAsync((DailyComplaint?)null);

        // Act
        var result = await _service.GetDailyComplaintByConstituencyNameAsync("Nowhere");

        // Assert
        Assert.Null(result);
    }

    private static DailyComplaint CreateDailyComplaint(int id, int constituencyId, int leaderId, bool isCompleted) =>
        new DailyComplaint
        {
            DailyComplaintId = id,
            ConstituencyId = constituencyId,
            LeaderId = leaderId,
            IsCompleted = isCompleted
        };

    private static DailyComplaintDto CreateDailyComplaintDto(DailyComplaint dc) =>
        new DailyComplaintDto
        {
            DailyComplaintId = dc.DailyComplaintId,
            ConstituencyId = dc.ConstituencyId,
            LeaderId = dc.LeaderId,
            IsCompleted = dc.IsCompleted,
            DateCreated = dc.DateCreated,
            DateUpdated = dc.DateUpdated
        };

    private static User CreateUser(int id, int constituencyId, string email, int reputation = 20) =>
        new User
        {
            UserId = id,
            ConstituencyId = constituencyId,
            Email = email,
            Reputation = reputation
        };
}
