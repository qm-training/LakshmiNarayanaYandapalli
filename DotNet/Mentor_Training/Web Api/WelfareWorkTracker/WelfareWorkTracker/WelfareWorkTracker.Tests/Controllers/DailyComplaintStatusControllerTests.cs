namespace WelfareWorkTracker.Tests.Controllers;
public class DailyComplaintStatusControllerTests
{
    private readonly Mock<IDailyComplaintStatusService> _mockService;
    private readonly DailyComplaintStatusController _controller;

    public DailyComplaintStatusControllerTests()
    {
        _mockService = new Mock<IDailyComplaintStatusService>();
        _controller = new DailyComplaintStatusController(_mockService.Object);
    }

    [Fact]
    public async Task UpdateDailyComplaintStatusByLeader_WithValidInput_ReturnsOk()
    {
        // Arrange
        AttachUser(_controller, "Leader");
        var vm = CreateVm(dailyComplaintId: 10, status: 2);
        var dto = new DailyComplaintStatusDto { DailyComplaintStatusId = 1, DailyComplaintId = 10, Status = 2 };
        _mockService.Setup(s => s.UpdateDailyComplaintStatusByLeaderAsync(vm)).ReturnsAsync(dto);

        // Act
        var result = await _controller.UpdateDailyComplaintStatusByLeader(vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task UpdateDailyComplaintStatusByLeader_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        AttachUser(_controller, "Leader");
        var vm = CreateVm(dailyComplaintId: 404, status: 3);
        _mockService.Setup(s => s.UpdateDailyComplaintStatusByLeaderAsync(vm)).ReturnsAsync((DailyComplaintStatusDto?)null);

        // Act
        var result = await _controller.UpdateDailyComplaintStatusByLeader(vm);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No daily complaint found with complaintId 404", nf.Value);
    }

    private static DailyComplaintStatusVm CreateVm(int dailyComplaintId, int status)
        => new DailyComplaintStatusVm { DailyComplaintId = dailyComplaintId, Status = status };

    private static void AttachUser(ControllerBase controller, string role)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("Id", "1"),
            new Claim(ClaimTypes.Role, role)
        }, "TestAuth");
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }
}