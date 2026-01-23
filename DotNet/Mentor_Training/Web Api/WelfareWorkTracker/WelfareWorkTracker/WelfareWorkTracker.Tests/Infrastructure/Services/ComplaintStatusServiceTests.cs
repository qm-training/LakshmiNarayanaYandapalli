namespace WelfareWorkTracker.Tests.Infrastructure.Services;
public class ComplaintStatusServiceTests
{
    private readonly Mock<IComplaintStatusRepository> _mockComplaintStatusRepository;
    private readonly Mock<IComplaintRepository> _mockComplaintRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IConstituencyRepository> _mockConstituencyRepository;
    private readonly Mock<IClaimsService> _mockClaimsService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEmailTemplateRepository> _mockEmailTemplateRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly ComplaintStatusService _complaintStatusService;

    public ComplaintStatusServiceTests()
    {
        _mockComplaintStatusRepository = new Mock<IComplaintStatusRepository>();
        _mockComplaintRepository = new Mock<IComplaintRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockConstituencyRepository = new Mock<IConstituencyRepository>();
        _mockClaimsService = new Mock<IClaimsService>();
        _mockMapper = new Mock<IMapper>();
        _mockEmailTemplateRepository = new Mock<IEmailTemplateRepository>();
        _mockEmailService = new Mock<IEmailService>();

        _complaintStatusService = new ComplaintStatusService(
            _mockComplaintStatusRepository.Object,
            _mockComplaintRepository.Object,
            _mockUserRepository.Object,
            _mockConstituencyRepository.Object,
            _mockClaimsService.Object,
            _mockMapper.Object,
            _mockEmailTemplateRepository.Object,
            _mockEmailService.Object);
    }

    [Fact]
    public async Task AddComplaintStatusByAdminAsync_WithValidBacklogFlow_ReturnsComplaintStatusDtoAndSendsEmails()
    {
        // Arrange
        var adminUserId = 10;
        var complaintId = 1;
        var constituencyId = 100;

        var vm = new StatusByAdminVm
        {
            ComplaintId = complaintId,
            Status = (int)Status.Backlog
        };

        var adminUser = CreateUser(adminUserId, constituencyId, "adminrep@test.com");
        var complaint = CreateComplaint(complaintId, constituencyId, citizenId: 5, leaderId: 20, "Test Complaint");
        var leader = CreateUser(20, constituencyId, "leader@test.com", reputation: 50);
        var citizen = CreateUser(5, constituencyId, "citizen@test.com");
        var complaintsInConstituency = new List<Complaint> { complaint };

        var currentStatus = CreateComplaintStatus(complaintId, (int)Status.Valid);
        var newBacklogStatus = CreateComplaintStatus(complaintId, (int)Status.Backlog);

        var citizenTemplate = CreateEmailTemplate(1, Constants.EmailTemplates.ComplaintCitizenBacklog);
        var leaderTemplate = CreateEmailTemplate(2, Constants.EmailTemplates.ComplaintLeaderBacklog);

        var expectedDto = new ComplaintStatusDto { ComplaintId = complaintId, Status = (int)Status.Backlog };

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(adminUserId);

        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(adminUserId))
            .ReturnsAsync(adminUser);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockUserRepository
            .Setup(r => r.GetLeaderByConstituencyIdAsync(complaint.ConstituencyId))
            .ReturnsAsync(leader);

        _mockConstituencyRepository
            .Setup(r => r.GetConstituencyNameByIdAsync(adminUser.ConstituencyId))
            .ReturnsAsync("Test Constituency");

        _mockComplaintRepository
            .Setup(r => r.GetComplaintsByConstituency("Test Constituency"))
            .ReturnsAsync(complaintsInConstituency);

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(It.Is<int?>(id => id == complaintId), It.IsAny<int?>()))
            .ReturnsAsync(currentStatus);

        _mockComplaintStatusRepository
            .Setup(r => r.AddComplaintStatusAsync(It.IsAny<ComplaintStatus>()))
            .ReturnsAsync(newBacklogStatus);

        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(complaint.CitizenId))
            .ReturnsAsync(citizen);

        _mockEmailTemplateRepository
            .Setup(r => r.GetByNameAsync(Constants.EmailTemplates.ComplaintLeaderBacklog))
            .ReturnsAsync(leaderTemplate);

        _mockEmailTemplateRepository
            .Setup(r => r.GetByNameAsync(Constants.EmailTemplates.ComplaintCitizenBacklog))
            .ReturnsAsync(citizenTemplate);

        _mockEmailService
            .Setup(s => s.SendEmailAsync(It.IsAny<EmailVm>()))
            .ReturnsAsync(true);

        _mockMapper
            .Setup(m => m.Map<ComplaintStatusDto>(It.IsAny<ComplaintStatus>()))
            .Returns(expectedDto);

        // Act
        var result = await _complaintStatusService.AddComplaintStatusByAdminAsync(vm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(complaintId, result!.ComplaintId);
        Assert.Equal((int)Status.Backlog, result.Status);

        _mockComplaintStatusRepository.Verify(r => r.AddComplaintStatusAsync(It.IsAny<ComplaintStatus>()), Times.Once);
        _mockEmailService.Verify(s => s.SendEmailAsync(It.IsAny<EmailVm>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddComplaintStatusByAdminAsync_WhenLeaderReputationTooLow_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var adminUserId = 10;
        var complaintId = 1;
        var constituencyId = 100;

        var vm = new StatusByAdminVm
        {
            ComplaintId = complaintId,
            Status = (int)Status.Backlog
        };

        var adminUser = CreateUser(adminUserId, constituencyId, "adminrep@test.com");
        var complaint = CreateComplaint(complaintId, constituencyId, citizenId: 5, leaderId: 20, "Test Complaint");
        var leader = CreateUser(20, constituencyId, "leader@test.com", reputation: 10);

        var complaintsInConstituency = new List<Complaint> { complaint };

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(adminUserId);

        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(adminUserId))
            .ReturnsAsync(adminUser);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockUserRepository
            .Setup(r => r.GetLeaderByConstituencyIdAsync(complaint.ConstituencyId))
            .ReturnsAsync(leader);

        _mockConstituencyRepository
            .Setup(r => r.GetConstituencyNameByIdAsync(adminUser.ConstituencyId))
            .ReturnsAsync("Test Constituency");

        _mockComplaintRepository
            .Setup(r => r.GetComplaintsByConstituency("Test Constituency"))
            .ReturnsAsync(complaintsInConstituency);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintStatusService.AddComplaintStatusByAdminAsync(vm));

        // Assert
        Assert.Equal("Leader is not eligible", exception.Message);
        _mockComplaintStatusRepository.Verify(r => r.AddComplaintStatusAsync(It.IsAny<ComplaintStatus>()), Times.Never);
    }

    [Fact]
    public async Task AddComplaintStatusByAdminAsync_WhenComplaintNotInAdminConstituency_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var adminUserId = 10;
        var complaintId = 1;
        var constituencyId = 100;

        var vm = new StatusByAdminVm
        {
            ComplaintId = complaintId,
            Status = (int)Status.Backlog
        };

        var adminUser = CreateUser(adminUserId, constituencyId, "adminrep@test.com");
        var complaint = CreateComplaint(complaintId, constituencyId, citizenId: 5, leaderId: 20, "Test Complaint");
        var leader = CreateUser(20, constituencyId, "leader@test.com", reputation: 50);

        var complaintsInConstituency = new List<Complaint>(); // Empty, so not found

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(adminUserId);

        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(adminUserId))
            .ReturnsAsync(adminUser);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockUserRepository
            .Setup(r => r.GetLeaderByConstituencyIdAsync(complaint.ConstituencyId))
            .ReturnsAsync(leader);

        _mockConstituencyRepository
            .Setup(r => r.GetConstituencyNameByIdAsync(adminUser.ConstituencyId))
            .ReturnsAsync("Test Constituency");

        _mockComplaintRepository
            .Setup(r => r.GetComplaintsByConstituency("Test Constituency"))
            .ReturnsAsync(complaintsInConstituency);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintStatusService.AddComplaintStatusByAdminAsync(vm));

        // Assert
        Assert.Equal("You can validate only complaints from your constituency.", exception.Message);
        _mockComplaintStatusRepository.Verify(r => r.AddComplaintStatusAsync(It.IsAny<ComplaintStatus>()), Times.Never);
    }

    [Fact]
    public async Task AddComplaintStatusByAdminRepAsync_WithComplaintOutsideConstituency_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var adminRepId = 10;
        var constituencyId = 100;

        var vm = new StatusByAdminRepVm
        {
            ComplaintId = 1,
            ReferenceNumber = 0,
            Status = (int)Status.Valid
        };

        var adminRep = CreateUser(adminRepId, constituencyId, "adminrep@test.com");

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(adminRepId);

        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(adminRepId))
            .ReturnsAsync(adminRep);

        _mockConstituencyRepository
            .Setup(r => r.GetConstituencyNameByIdAsync(adminRep.ConstituencyId))
            .ReturnsAsync("Test Constituency");

        _mockComplaintRepository
            .Setup(r => r.GetComplaintsByConstituency("Test Constituency"))
            .ReturnsAsync(new List<Complaint>());

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintStatusService.AddComplaintStatusByAdminRepAsync(vm));

        // Assert
        Assert.Equal("You can validate only complaints from your constituency.", exception.Message);
    }

    [Fact]
    public async Task GetComplaintStatusByComplaintId_WithExistingStatus_ReturnsDto()
    {
        // Arrange
        var complaintId = 1;
        var status = CreateComplaintStatus(complaintId, (int)Status.Backlog);
        var dto = new ComplaintStatusDto { ComplaintId = complaintId, Status = status.Status };

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(It.Is<int?>(id => id == complaintId), It.IsAny<int?>()))
            .ReturnsAsync(status);

        _mockMapper
            .Setup(m => m.Map<ComplaintStatusDto>(status))
            .Returns(dto);

        // Act
        var result = await _complaintStatusService.GetComplaintStatusByComplaintId(complaintId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(complaintId, result!.ComplaintId);
    }

    [Fact]
    public async Task GetComplaintStatusByComplaintId_WhenStatusIsNull_ReturnsNull()
    {
        // Arrange
        var complaintId = 1;

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(It.Is<int?>(id => id == complaintId), It.IsAny<int?>()))
            .ReturnsAsync((ComplaintStatus?)null);

        // Act
        var result = await _complaintStatusService.GetComplaintStatusByComplaintId(complaintId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetComplaintStatusByIdAsync_WithExistingStatus_ReturnsDto()
    {
        // Arrange
        var complaintStatusId = 5;
        var status = CreateComplaintStatus(1, (int)Status.Backlog);
        var dto = new ComplaintStatusDto { ComplaintId = status.ComplaintId, Status = status.Status };

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(null, complaintStatusId))
            .ReturnsAsync(status);

        _mockMapper
            .Setup(m => m.Map<ComplaintStatusDto>(status))
            .Returns(dto);

        // Act
        var result = await _complaintStatusService.GetComplaintStatusByIdAsync(complaintStatusId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(status.ComplaintId, result!.ComplaintId);
    }

    [Fact]
    public async Task GetComplaintStatusByIdAsync_WhenStatusIsNull_ReturnsNull()
    {
        // Arrange
        var complaintStatusId = 5;

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(null, complaintStatusId))
            .ReturnsAsync((ComplaintStatus?)null);

        // Act
        var result = await _complaintStatusService.GetComplaintStatusByIdAsync(complaintStatusId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetComplaintStatusHistoryAsync_WithStatuses_ReturnsMappedList()
    {
        // Arrange
        var complaintId = 1;
        var statuses = new List<ComplaintStatus>
        {
            CreateComplaintStatus(complaintId, (int)Status.Backlog),
            CreateComplaintStatus(complaintId, (int)Status.InProgress)
        };

        var dtos = new List<ComplaintStatusDto>
        {
            new ComplaintStatusDto { ComplaintId = complaintId, Status = (int)Status.Backlog },
            new ComplaintStatusDto { ComplaintId = complaintId, Status = (int)Status.InProgress }
        };

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusHistoryByComplaintIdAsync(complaintId))
            .ReturnsAsync(statuses);

        _mockMapper
            .Setup(m => m.Map<List<ComplaintStatusDto>>(statuses))
            .Returns(dtos);

        // Act
        var result = await _complaintStatusService.GetComplaintStatusHistoryAsync(complaintId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Status == (int)Status.Backlog);
        Assert.Contains(result, s => s.Status == (int)Status.InProgress);
    }

    [Fact]
    public async Task UpdateComplaintStatusByLeaderAsync_WhenLeaderReputationTooLow_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var leaderUserId = 20;
        var constituencyId = 100;
        var complaintId = 1;

        var vm = new StatusByLeaderVm
        {
            ComplaintId = complaintId,
            Status = (int)Status.Approve,
            DeadlineDate = DateTime.UtcNow.AddDays(3)
        };

        var user = CreateUser(leaderUserId, constituencyId, "leader@test.com");
        var leader = CreateUser(leaderUserId, constituencyId, "leader@test.com", reputation: 10);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(leaderUserId);

        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(leaderUserId))
            .ReturnsAsync(user);

        _mockUserRepository
            .Setup(r => r.GetLeaderByConstituencyIdAsync(user.ConstituencyId))
            .ReturnsAsync(leader);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintStatusService.UpdateComplaintStatusByLeaderAsync(vm));

        // Assert
        Assert.Equal("You cannot approve as your reputation dropped below 15%", exception.Message);
    }

    [Fact]
    public async Task UpdateComplaintStatusByLeaderAsync_WithApproveOnNonBacklogStatus_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var leaderUserId = 20;
        var constituencyId = 100;
        var complaintId = 1;

        var vm = new StatusByLeaderVm
        {
            ComplaintId = complaintId,
            Status = (int)Status.Approve,
            DeadlineDate = DateTime.UtcNow.AddDays(3)
        };

        var user = CreateUser(leaderUserId, constituencyId, "leader@test.com");
        var leader = CreateUser(leaderUserId, constituencyId, "leader@test.com", reputation: 50);
        var complaint = CreateComplaint(complaintId, constituencyId, 5, leaderUserId, "Test Complaint");
        var complaintsInConstituency = new List<Complaint> { complaint };
        var currentStatus = CreateComplaintStatus(complaintId, (int)Status.InProgress);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(leaderUserId);

        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(leaderUserId))
            .ReturnsAsync(user);

        _mockUserRepository
            .Setup(r => r.GetLeaderByConstituencyIdAsync(user.ConstituencyId))
            .ReturnsAsync(leader);

        _mockConstituencyRepository
            .Setup(r => r.GetConstituencyNameByIdAsync(user.ConstituencyId))
            .ReturnsAsync("Test Constituency");

        _mockComplaintRepository
            .Setup(r => r.GetComplaintsByConstituency("Test Constituency"))
            .ReturnsAsync(complaintsInConstituency);

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(It.Is<int?>(id => id == complaintId), It.IsAny<int?>()))
            .ReturnsAsync(currentStatus);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintStatusService.UpdateComplaintStatusByLeaderAsync(vm));

        // Assert
        Assert.Equal("The complaint must be in 'Backlog' (4) status to be approved.", exception.Message);
    }

    [Fact]
    public async Task UpdateComplaintStatusByLeaderAsync_WithResolveOnNonInProgressStatus_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var leaderUserId = 20;
        var constituencyId = 100;
        var complaintId = 1;

        var vm = new StatusByLeaderVm
        {
            ComplaintId = complaintId,
            Status = (int)Status.Resolved,
        };

        var user = CreateUser(leaderUserId, constituencyId, "leader@test.com");
        var leader = CreateUser(leaderUserId, constituencyId, "leader@test.com", reputation: 50);
        var complaint = CreateComplaint(complaintId, constituencyId, 5, leaderUserId, "Test Complaint");
        var complaintsInConstituency = new List<Complaint> { complaint };
        var currentStatus = CreateComplaintStatus(complaintId, (int)Status.Backlog);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(leaderUserId);

        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(leaderUserId))
            .ReturnsAsync(user);

        _mockUserRepository
            .Setup(r => r.GetLeaderByConstituencyIdAsync(user.ConstituencyId))
            .ReturnsAsync(leader);

        _mockConstituencyRepository
            .Setup(r => r.GetConstituencyNameByIdAsync(user.ConstituencyId))
            .ReturnsAsync("Test Constituency");

        _mockComplaintRepository
            .Setup(r => r.GetComplaintsByConstituency("Test Constituency"))
            .ReturnsAsync(complaintsInConstituency);

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(It.Is<int?>(id => id == complaintId), It.IsAny<int?>()))
            .ReturnsAsync(currentStatus);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintStatusService.UpdateComplaintStatusByLeaderAsync(vm));

        // Assert
        Assert.Equal("Complaint must be in 'InProgress' (7) status to resolve.", exception.Message);
    }

    private static User CreateUser(int userId, int constituencyId, string email, double reputation = 50)
    {
        return new User
        {
            UserId = userId,
            ConstituencyId = constituencyId,
            Email = email,
            FullName = $"User {userId}",
            Reputation = reputation
        };
    }

    private static Complaint CreateComplaint(int complaintId, int constituencyId, int citizenId, int leaderId, string title)
    {
        return new Complaint
        {
            ComplaintId = complaintId,
            ConstituencyId = constituencyId,
            CitizenId = citizenId,
            LeaderId = leaderId,
            Title = title,
            Description = "Test Description",
            ConstituencyName = "Test Constituency",
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };
    }

    private static ComplaintStatus CreateComplaintStatus(int complaintId, int status)
    {
        return new ComplaintStatus
        {
            ComplaintId = complaintId,
            Status = status,
            AttemptNumber = 1,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            OpenedDate = DateTime.UtcNow
        };
    }

    private static EmailTemplate CreateEmailTemplate(int id, string name)
    {
        return new EmailTemplate
        {
            Id = id,
            Name = name,
            Subject = "Test Subject",
            Body = "Test Body",
            IsActive = true,
            DateCreated = DateTime.UtcNow
        };
    }
}
