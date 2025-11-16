namespace WelfareWorkTracker.Tests.Controllers;
public class FeedbackControllerTests
{
    private readonly Mock<IFeedbackService> _mockService;
    private readonly FeedbackController _controller;

    public FeedbackControllerTests()
    {
        _mockService = new Mock<IFeedbackService>();
        _controller = new FeedbackController(_mockService.Object);
    }

    [Fact]
    public async Task AddFeedback_WithValidInput_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        AttachUser(_controller, "Citizen");
        var vm = CreateVm("Good job", true, 1, null);
        var dto = CreateDto(1, "Good job", true, 1, null, 42);
        _mockService.Setup(s => s.AddFeedbackAsync(vm)).ReturnsAsync(dto);

        // Act
        var result = await _controller.AddFeedback(vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal("Feedback added sucessfully", ok.Value);
    }

    [Fact]
    public async Task AddFeedback_WhenServiceReturnsNull_ReturnsBadRequest()
    {
        // Arrange
        AttachUser(_controller, "Citizen");
        var vm = CreateVm("Oops", false, 1, null);
        _mockService.Setup(s => s.AddFeedbackAsync(vm)).ReturnsAsync((FeedbackDto?)null);

        // Act
        var result = await _controller.AddFeedback(vm);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to add feedback, Please retry!", bad.Value);
    }

    [Fact]
    public async Task GetFeedbackById_WithAllComplaintsAndMatches_ReturnsOkWithList()
    {
        // Arrange
        var list = new List<FeedbackDto>
        {
            CreateDto(1, "A", true, 10, null, 5),
            CreateDto(2, "B", false, 11, null, 6)
        };
        _mockService.Setup(s => s.GetAllFeedbacksAsync(10, null)).ReturnsAsync(list);

        // Act
        var result = await _controller.GetFeedbackById(10, null, isAllComplaints: true);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(list, ok.Value);
    }

    [Fact]
    public async Task GetFeedbackById_WithAllComplaintsAndNoMatches_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllFeedbacksAsync(99, null)).ReturnsAsync((List<FeedbackDto>?)null);

        // Act
        var result = await _controller.GetFeedbackById(99, null, isAllComplaints: true);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<FeedbackDto>>(ok.Value);
        Assert.Empty(value);
    }

    [Fact]
    public async Task GetFeedbackById_WithSingleComplaintAndMatch_ReturnsOkWithSingleItemList()
    {
        // Arrange
        var dto = CreateDto(7, "Only mine", true, 22, null, 42);
        _mockService.Setup(s => s.GetFeedbackByUserAsync(22, null)).ReturnsAsync(dto);

        // Act
        var result = await _controller.GetFeedbackById(22, null, isAllComplaints: false);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<FeedbackDto>>(ok.Value);
        Assert.Single(list);
        Assert.Equal(dto, list[0]);
    }

    [Fact]
    public async Task GetFeedbackById_WithSingleComplaintAndNoMatch_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetFeedbackByUserAsync(null, 55)).ReturnsAsync((FeedbackDto?)null);

        // Act
        var result = await _controller.GetFeedbackById(null, 55, isAllComplaints: false);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    private static FeedbackVm CreateVm(string? message, bool isSatisfied, int? complaintId, int? dailyComplaintId)
        => new FeedbackVm
        {
            FeedbackMessage = message,
            IsSatisfied = isSatisfied,
            ComplaintId = complaintId,
            DailyComplaintId = dailyComplaintId
        };

    private static FeedbackDto CreateDto(
        int id,
        string? message,
        bool isSatisfied,
        int? complaintId,
        int? dailyComplaintId,
        int citizenId)
        => new FeedbackDto
        {
            CitizenFeedbackId = id,
            FeedbackMessage = message,
            IsSatisfied = isSatisfied,
            ComplaintId = complaintId,
            DailyComplaintId = dailyComplaintId,
            CitizenId = citizenId,
            DateCreated = System.DateTime.UtcNow,
            DateUpdated = System.DateTime.UtcNow
        };

    private static void AttachUser(ControllerBase controller, string role)
    {
        var claims = new List<Claim>
        {
            new Claim("Id", "42"),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }
}
