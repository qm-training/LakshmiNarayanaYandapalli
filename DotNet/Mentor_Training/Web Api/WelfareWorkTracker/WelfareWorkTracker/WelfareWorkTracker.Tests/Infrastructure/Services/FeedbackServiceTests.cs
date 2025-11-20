namespace WelfareWorkTracker.Tests.Infrastructure.Services;
public class FeedbackServiceTests
{
    private readonly Mock<IFeedbackRepository> _mockFeedbackRepo;
    private readonly Mock<IComplaintRepository> _mockComplaintRepo;
    private readonly Mock<IDailyComplaintRepository> _mockDailyComplaintRepo;
    private readonly Mock<IComplaintStatusRepository> _mockComplaintStatusRepo;
    private readonly Mock<IDailyComplaintStatusRepository> _mockDailyComplaintStatusRepo;
    private readonly Mock<IClaimsService> _mockClaims;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IEmailTemplateRepository> _mockEmailTemplateRepo;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IMapper> _mockMapper;

    private readonly FeedbackService _service;

    public FeedbackServiceTests()
    {
        _mockFeedbackRepo = new Mock<IFeedbackRepository>();
        _mockComplaintRepo = new Mock<IComplaintRepository>();
        _mockDailyComplaintRepo = new Mock<IDailyComplaintRepository>();
        _mockComplaintStatusRepo = new Mock<IComplaintStatusRepository>();
        _mockDailyComplaintStatusRepo = new Mock<IDailyComplaintStatusRepository>();
        _mockClaims = new Mock<IClaimsService>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockEmailTemplateRepo = new Mock<IEmailTemplateRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockMapper = new Mock<IMapper>();

        _service = new FeedbackService(
            _mockFeedbackRepo.Object,
            _mockComplaintRepo.Object,
            _mockDailyComplaintRepo.Object,
            _mockComplaintStatusRepo.Object,
            _mockDailyComplaintStatusRepo.Object,
            _mockClaims.Object,
            _mockUserRepo.Object,
            _mockEmailTemplateRepo.Object,
            _mockEmailService.Object,
            _mockMapper.Object
        );

        _mockEmailTemplateRepo.Setup(x => x.GetByNameAsync(It.IsAny<string>()))
                              .ReturnsAsync(new EmailTemplate { Id = 99, Name = "T" });
        _mockEmailService.Setup(x => x.SendEmailAsync(It.IsAny<EmailVm>())).ReturnsAsync(true);
    }

    [Fact]
    public async Task AddFeedbackAsync_WithDailyComplaintResolved_AddsAndReturnsDto()
    {
        // Arrange
        var userId = 20;
        var daily = new DailyComplaint { DailyComplaintId = 11, ConstituencyId = 5, LeaderId = 2 };
        var dailyStatus = new DailyComplaintStatus { DailyComplaintId = 11, Status = (int)Status.Resolved };
        var user = CreateUser(userId, 5, "x@test.com");

        var vm = new FeedbackVm { DailyComplaintId = 11, FeedbackMessage = "OK", IsSatisfied = false };

        _mockClaims.Setup(c => c.GetUserIdFromClaimsAsync()).ReturnsAsync(userId);
        _mockComplaintRepo.Setup(r => r.GetComplaintByComplaintIdAsync(It.IsAny<int>())).ReturnsAsync((Complaint?)null);
        _mockDailyComplaintRepo.Setup(r => r.GetDailyComplaintByIdAsync(11)).ReturnsAsync(daily);
        _mockUserRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _mockFeedbackRepo.Setup(r => r.GetFeedbackByUserAsync(userId, null, 11)).ReturnsAsync((ComplaintFeedback?)null);
        _mockDailyComplaintStatusRepo.Setup(r => r.GetDailyComplaintStatusAsync(11, null)).ReturnsAsync(dailyStatus);

        var saved = new ComplaintFeedback { CitizenId = userId, DailyComplaintId = 11, FeedbackMessage = "OK", IsSatisfied = false };
        _mockFeedbackRepo.Setup(r => r.AddFeedbackAsync(It.IsAny<ComplaintFeedback>()))
                         .ReturnsAsync(saved);

        _mockMapper.Setup(m => m.Map<FeedbackDto>(saved))
                   .Returns(new FeedbackDto { DailyComplaintId = 11, FeedbackMessage = "OK", IsSatisfied = false });

        // Act
        var result = await _service.AddFeedbackAsync(vm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(11, result.DailyComplaintId);
        Assert.False(result.IsSatisfied);
    }

    [Fact]
    public async Task AddFeedbackAsync_WithNoIds_ThrowsBadRequest()
    {
        // Arrange
        var vm = new FeedbackVm { FeedbackMessage = "x" };

        // Act
        var act = () => _service.AddFeedbackAsync(vm);

        // Assert
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(act);
        Assert.Equal((int)HttpStatusCode.BadRequest, ex.StatusCode);
    }

    [Fact]
    public async Task AddFeedbackAsync_WithBothIds_ThrowsBadRequest()
    {
        // Arrange
        var vm = new FeedbackVm { ComplaintId = 1, DailyComplaintId = 2, FeedbackMessage = "x" };

        // Act
        var act = () => _service.AddFeedbackAsync(vm);

        // Assert
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(act);
        Assert.Equal((int)HttpStatusCode.BadRequest, ex.StatusCode);
    }

    [Fact]
    public async Task AddFeedbackAsync_WithDuplicateFeedback_ThrowsBadRequest()
    {
        // Arrange
        var userId = 5;
        var vm = new FeedbackVm { ComplaintId = 3, FeedbackMessage = "dup" };

        _mockClaims.Setup(c => c.GetUserIdFromClaimsAsync()).ReturnsAsync(userId);
        _mockComplaintRepo.Setup(r => r.GetComplaintByComplaintIdAsync(3)).ReturnsAsync(CreateComplaint(3, 1, 77, "T"));
        _mockUserRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(CreateUser(userId, 1, "u@t.com"));
        _mockFeedbackRepo.Setup(r => r.GetFeedbackByUserAsync(userId, 3, null))
                         .ReturnsAsync(new ComplaintFeedback());

        // Act
        var act = () => _service.AddFeedbackAsync(vm);

        // Assert
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(act);
        Assert.Equal((int)HttpStatusCode.BadRequest, ex.StatusCode);
    }

    [Fact]
    public async Task AddFeedbackAsync_WithComplaintNotResolved_ThrowsBadRequest()
    {
        // Arrange
        var userId = 2;
        var vm = new FeedbackVm { ComplaintId = 10, FeedbackMessage = "meh" };

        _mockClaims.Setup(c => c.GetUserIdFromClaimsAsync()).ReturnsAsync(userId);
        _mockComplaintRepo.Setup(r => r.GetComplaintByComplaintIdAsync(10)).ReturnsAsync(CreateComplaint(10, 9, 40, "T"));
        _mockUserRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(CreateUser(userId, 9, "a@a.com"));
        _mockFeedbackRepo.Setup(r => r.GetFeedbackByUserAsync(userId, 10, null)).ReturnsAsync((ComplaintFeedback?)null);
        _mockComplaintStatusRepo.Setup(r => r.GetComplaintStatusAsync(10, null)).ReturnsAsync(new ComplaintStatus { ComplaintId = 10, Status = (int)Status.InProgress });

        // Act
        var act = () => _service.AddFeedbackAsync(vm);

        // Assert
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(act);
        Assert.Equal((int)HttpStatusCode.BadRequest, ex.StatusCode);
    }

    [Fact]
    public async Task GetAllFeedbacksAsync_WithComplaintId_ReturnsOrderedDtos()
    {
        // Arrange
        var userId = 9;
        var inputComplaintId = 50;

        var list = new List<ComplaintFeedback>
            {
                new() { CitizenId = 1, ComplaintId = inputComplaintId, FeedbackMessage = "a" },
                new() { CitizenId = userId, ComplaintId = inputComplaintId, FeedbackMessage = "b" },
                new() { CitizenId = 3, ComplaintId = inputComplaintId, FeedbackMessage = "c" },
            };

        _mockClaims.Setup(c => c.GetUserIdFromClaimsAsync()).ReturnsAsync(userId);
        _mockFeedbackRepo.Setup(r => r.GetAllFeedbacksAsync(inputComplaintId, null)).ReturnsAsync(list);

        _mockMapper.Setup(m => m.Map<FeedbackDto>(It.IsAny<ComplaintFeedback>()))
                   .Returns<ComplaintFeedback>(f => new FeedbackDto { CitizenId = f.CitizenId, FeedbackMessage = f.FeedbackMessage });

        // Act
        var result = await _service.GetAllFeedbacksAsync(inputComplaintId, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result!.Count);
        Assert.Equal(userId, result[0].CitizenId);
    }

    [Fact]
    public async Task GetAllFeedbacksAsync_WithNullResult_ReturnsNull()
    {
        // Arrange
        _mockClaims.Setup(c => c.GetUserIdFromClaimsAsync()).ReturnsAsync(1);
        _mockFeedbackRepo.Setup(r => r.GetAllFeedbacksAsync(null, 12)).ReturnsAsync((List<ComplaintFeedback>)null!);

        // Act
        var result = await _service.GetAllFeedbacksAsync(null, 12);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllFeedbacksAsync_WithNoIds_ThrowsBadRequest()
    {
        // Arrange & Act
        var act = () => _service.GetAllFeedbacksAsync(null, null);

        // Assert
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(act);
        Assert.Equal((int)HttpStatusCode.BadRequest, ex.StatusCode);
    }


    [Fact]
    public async Task GetFeedbackByUserAsync_WithExisting_ReturnsDto()
    {
        // Arrange
        var userId = 7;
        var stored = new ComplaintFeedback { CitizenId = userId, ComplaintId = 77, FeedbackMessage = "ok" };

        _mockClaims.Setup(c => c.GetUserIdFromClaimsAsync()).ReturnsAsync(userId);
        _mockFeedbackRepo.Setup(r => r.GetFeedbackByUserAsync(userId, 77, null)).ReturnsAsync(stored);
        _mockMapper.Setup(m => m.Map<FeedbackDto>(stored))
                   .Returns(new FeedbackDto { CitizenId = userId, ComplaintId = 77, FeedbackMessage = "ok" });

        // Act
        var result = await _service.GetFeedbackByUserAsync(77, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(77, result!.ComplaintId);
    }

    [Fact]
    public async Task GetFeedbackByUserAsync_WithMissing_ReturnsEmptyDto()
    {
        // Arrange
        _mockClaims.Setup(c => c.GetUserIdFromClaimsAsync()).ReturnsAsync(3);
        _mockFeedbackRepo.Setup(r => r.GetFeedbackByUserAsync(3, null, 5)).ReturnsAsync((ComplaintFeedback?)null);

        // Act
        var result = await _service.GetFeedbackByUserAsync(null, 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(default, result!.ComplaintId);
        Assert.Equal(default, result.DailyComplaintId);
    }

    [Fact]
    public async Task GetFeedbackByUserAsync_WithBothIds_ThrowsBadRequest()
    {
        // Arrange & Act
        var act = () => _service.GetFeedbackByUserAsync(1, 2);

        // Assert
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(act);
        Assert.Equal((int)HttpStatusCode.BadRequest, ex.StatusCode);
    }


    [Fact]
    public async Task EvaluateFeedback_LeaderLowReputation_NoAction()
    {
        // Arrange
        var complaint = CreateComplaint(1, 10, 50, "T");
        var status = new ComplaintStatus { ComplaintId = 1, Status = (int)Status.Resolved, DateUpdated = DateTime.UtcNow };
        var leader = CreateUser(2, 10, "l@test.com", reputation: 10.0); // <=15

        _mockComplaintRepo.Setup(r => r.GetComplaintByComplaintIdAsync(1)).ReturnsAsync(complaint);
        _mockComplaintStatusRepo.Setup(r => r.GetComplaintStatusAsync(1, null)).ReturnsAsync(status);
        _mockUserRepo.Setup(r => r.GetLeaderByConstituencyIdAsync(10)).ReturnsAsync(leader);

        // Act
        await _service.EvaluateFeedback(1);

        // Assert
        _mockUserRepo.Verify(r => r.UpdateLeaderReputationAsync(It.IsAny<int>(), It.IsAny<double>()), Times.Never);
        _mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<EmailVm>()), Times.Never);
    }

    [Fact]
    public async Task EvaluateFeedback_Resolved_Satisfied_ClosesAndRaisesReputationAndSendsEmails()
    {
        // Arrange
        var complaint = CreateComplaint(1, 10, 77, "Park cleaning");
        var status = new ComplaintStatus { ComplaintId = 1, Status = (int)Status.Resolved, DateUpdated = DateTime.UtcNow.AddMinutes(-1) };
        var leader = CreateUser(2, 10, "leader@test.com", reputation: 50.0);
        var citizen = CreateUser(77, 10, "citizen@test.com");

        _mockComplaintRepo.Setup(r => r.GetComplaintByComplaintIdAsync(1)).ReturnsAsync(complaint);
        _mockComplaintStatusRepo.Setup(r => r.GetComplaintStatusAsync(1, null)).ReturnsAsync(status);
        _mockUserRepo.Setup(r => r.GetLeaderByConstituencyIdAsync(10)).ReturnsAsync(leader);
        _mockUserRepo.Setup(r => r.GetCitizenCountInConstituencyAsync(10)).ReturnsAsync(1000);

        _mockFeedbackRepo.Setup(r => r.GetSatisfiedCount(1, null)).ReturnsAsync(100);
        _mockFeedbackRepo.Setup(r => r.GetUnSatisfiedCount(1, null)).ReturnsAsync(5);

        _mockUserRepo.Setup(r => r.GetUserByIdAsync(77)).ReturnsAsync(citizen);

        _mockComplaintStatusRepo.Setup(r => r.AddComplaintStatusAsync(It.IsAny<ComplaintStatus>()))
                                .ReturnsAsync((ComplaintStatus cs) => cs);

        // Act
        await _service.EvaluateFeedback(1);

        // Assert
        _mockComplaintStatusRepo.Verify(r => r.AddComplaintStatusAsync(It.Is<ComplaintStatus>(cs =>
            cs.ComplaintId == 1 && cs.Status == (int)Status.Closed)), Times.Once);

        _mockUserRepo.Verify(r => r.UpdateLeaderReputationAsync(leader.UserId, It.Is<double>(rep => rep > 50.0)), Times.AtLeastOnce);

        _mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<EmailVm>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task EvaluateFeedback_Resolved_Unsatisfied_AttemptsUnder4_ReopensAndReducesReputation()
    {
        // Arrange
        var complaint = CreateComplaint(1, 10, 77, "Street light");
        complaint.Attempts = 1; // will increment and stay <4
        var status = new ComplaintStatus { ComplaintId = 1, Status = (int)Status.Resolved, DateUpdated = DateTime.UtcNow.AddMinutes(-1) };
        var leader = CreateUser(2, 10, "leader@test.com", reputation: 60.0);
        var citizen = CreateUser(77, 10, "citizen@test.com");

        _mockComplaintRepo.Setup(r => r.GetComplaintByComplaintIdAsync(1)).ReturnsAsync(complaint);
        _mockComplaintStatusRepo.Setup(r => r.GetComplaintStatusAsync(1, null)).ReturnsAsync(status);
        _mockUserRepo.Setup(r => r.GetLeaderByConstituencyIdAsync(10)).ReturnsAsync(leader);
        _mockUserRepo.Setup(r => r.GetCitizenCountInConstituencyAsync(10)).ReturnsAsync(1000);

        _mockFeedbackRepo.Setup(r => r.GetSatisfiedCount(1, null)).ReturnsAsync(5);
        _mockFeedbackRepo.Setup(r => r.GetUnSatisfiedCount(1, null)).ReturnsAsync(100);

        _mockUserRepo.Setup(r => r.GetUserByIdAsync(77)).ReturnsAsync(citizen);
        _mockComplaintStatusRepo.Setup(r => r.AddComplaintStatusAsync(It.IsAny<ComplaintStatus>()))
                                .ReturnsAsync((ComplaintStatus cs) => cs);

        // Act
        await _service.EvaluateFeedback(1);

        // Assert
        _mockComplaintStatusRepo.Verify(r => r.AddComplaintStatusAsync(It.Is<ComplaintStatus>(cs =>
            cs.ComplaintId == 1 && cs.Status == (int)Status.Reopened)), Times.Once);

        _mockComplaintRepo.Verify(r => r.UpdateComplaintByIdAsync(It.Is<Complaint>(c => c.ComplaintId == 1 && c.Attempts >= 2)), Times.Once);

        _mockUserRepo.Verify(r => r.UpdateLeaderReputationAsync(leader.UserId, It.IsAny<double>()), Times.AtLeastOnce);

        _mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<EmailVm>()), Times.AtLeastOnce);
    }


    [Fact]
    public async Task EvaluateDailyComplaintFeedback_Resolved_Satisfied_IncreasesReputation()
    {
        // Arrange
        var dc = new DailyComplaint { DailyComplaintId = 5, ConstituencyId = 3, LeaderId = 2 };
        var dcs = new DailyComplaintStatus { DailyComplaintId = 5, Status = (int)Status.Resolved };
        var leader = CreateUser(2, 3, "l@test.com", reputation: 40.0);

        _mockDailyComplaintRepo.Setup(r => r.GetTodaysDailyComplaintsAsync())
                               .ReturnsAsync(new List<DailyComplaint> { dc });

        _mockDailyComplaintStatusRepo.Setup(r => r.GetDailyComplaintStatusAsync(5, null))
                                     .ReturnsAsync(dcs);

        _mockUserRepo.Setup(r => r.GetLeaderByConstituencyIdAsync(3)).ReturnsAsync(leader);
        _mockUserRepo.Setup(r => r.GetCitizenCountInConstituencyAsync(3)).ReturnsAsync(1000);

        _mockFeedbackRepo.Setup(r => r.GetSatisfiedCount(null, 5)).ReturnsAsync(200);
        _mockFeedbackRepo.Setup(r => r.GetUnSatisfiedCount(null, 5)).ReturnsAsync(10);

        // Act
        await _service.EvaluateDailyComplaintFeedback();

        // Assert
        _mockUserRepo.Verify(r => r.UpdateLeaderReputationAsync(leader.UserId, It.Is<double>(rep => rep > 40.0)), Times.Once);
    }

    [Fact]
    public async Task EvaluateDailyComplaintFeedback_Resolved_Unsatisfied_MoreThan3Unresolved_ReducesByFive()
    {
        // Arrange
        var dc = new DailyComplaint { DailyComplaintId = 7, ConstituencyId = 6, LeaderId = 4 };
        var dcs = new DailyComplaintStatus { DailyComplaintId = 7, Status = (int)Status.Resolved };
        var leader = CreateUser(4, 6, "l@test.com", reputation: 30.0);

        _mockDailyComplaintRepo.Setup(r => r.GetTodaysDailyComplaintsAsync())
                               .ReturnsAsync(new List<DailyComplaint> { dc });

        _mockDailyComplaintStatusRepo.Setup(r => r.GetDailyComplaintStatusAsync(7, null))
                                     .ReturnsAsync(dcs);

        _mockUserRepo.Setup(r => r.GetLeaderByConstituencyIdAsync(6)).ReturnsAsync(leader);
        _mockUserRepo.Setup(r => r.GetCitizenCountInConstituencyAsync(6)).ReturnsAsync(1000);

        _mockFeedbackRepo.Setup(r => r.GetSatisfiedCount(null, 7)).ReturnsAsync(10);
        _mockFeedbackRepo.Setup(r => r.GetUnSatisfiedCount(null, 7)).ReturnsAsync(200);

        _mockDailyComplaintStatusRepo.Setup(r => r.GetUnresolvedComplaintsOfLeader(leader.UserId, Status.Unresolved))
                                     .ReturnsAsync(new List<DailyComplaintStatus> { new(), new(), new(), new() });

        // Act
        await _service.EvaluateDailyComplaintFeedback();

        // Assert
        _mockUserRepo.Verify(r => r.UpdateLeaderReputationAsync(leader.UserId, It.Is<double>(rep => rep <= 25.0)), Times.Once);
    }

    private static Complaint CreateComplaint(int complaintId, int constituencyId, int citizenId, string title) =>
        new Complaint
        {
            ComplaintId = complaintId,
            ConstituencyId = constituencyId,
            CitizenId = citizenId,
            Title = title,
            Attempts = 1,
            ConstituencyName = "X"
        };

    private static User CreateUser(int id, int constituencyId, string email, double reputation = 50.0) =>
        new User
        {
            UserId = id,
            ConstituencyId = constituencyId,
            Email = email,
            FullName = $"User{id}",
            Reputation = reputation
        };
}
