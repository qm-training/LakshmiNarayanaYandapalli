namespace WelfareWorkTracker.Tests.Controllers;
public class DailyComplaintControllerTests
{
    private readonly Mock<IDailyComplaintService> _mockService;
    private readonly DailyComplaintController _controller;

    public DailyComplaintControllerTests()
    {
        _mockService = new Mock<IDailyComplaintService>();
        _controller = new DailyComplaintController(_mockService.Object);
    }

    [Fact]
    public async Task GetDailyComplaintsById_WithExistingId_ReturnsOk()
    {
        // Arrange
        var dto = new DailyComplaintDto { DailyComplaintId = 1, ConstituencyId = 9, IsCompleted = false };
        _mockService.Setup(s => s.GetDailyComplaintByIdAsync(1)).ReturnsAsync(dto);

        // Act
        var result = await _controller.GetDailyComplaintsById(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetDailyComplaintsById_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetDailyComplaintByIdAsync(404)).ReturnsAsync((DailyComplaintDto?)null);

        // Act
        var result = await _controller.GetDailyComplaintsById(404);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No Daily Complaint found with complaintId: 404", nf.Value);
    }

    [Fact]
    public async Task UpdateDailyComplaintById_WithExistingId_ReturnsOk()
    {
        // Arrange
        var vm = CreateVm(isCompleted: true, constituencyId: 5);
        var dto = new DailyComplaintDto { DailyComplaintId = 7, ConstituencyId = vm.ConstituencyId, IsCompleted = vm.IsCompleted };
        _mockService.Setup(s => s.UpdateDailyComplaintAsync(7, vm)).ReturnsAsync(dto);

        // Act
        var result = await _controller.UpdateDailyComplaintById(7, vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task UpdateDailyComplaintById_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var vm = CreateVm();
        _mockService.Setup(s => s.UpdateDailyComplaintAsync(999, vm)).ReturnsAsync((DailyComplaintDto?)null);

        // Act
        var result = await _controller.UpdateDailyComplaintById(999, vm);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No Daily Complaint found with complaintId: 999", nf.Value);
    }

    [Fact]
    public async Task GetDailyComplaints_WithData_ReturnsOkWithList()
    {
        // Arrange
        var list = new List<DailyComplaintDto>
        {
            new DailyComplaintDto { DailyComplaintId = 1, ConstituencyId = 1, IsCompleted = false },
            new DailyComplaintDto { DailyComplaintId = 2, ConstituencyId = 2, IsCompleted = true }
        };
        _mockService.Setup(s => s.GetDailyComplaintsAsync()).ReturnsAsync(list);

        // Act
        var result = await _controller.GetDailyComplaints();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(list, ok.Value);
    }

    [Fact]
    public async Task GetDailyComplaints_WhenEmpty_ReturnsOkWithEmptyList()
    {
        // Arrange
        var empty = new List<DailyComplaintDto>();
        _mockService.Setup(s => s.GetDailyComplaintsAsync()).ReturnsAsync(empty);

        // Act
        var result = await _controller.GetDailyComplaints();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(empty, ok.Value);
    }

    [Fact]
    public async Task GetDailyComplaintByLeaderId_WithExisting_ReturnsOk()
    {
        // Arrange
        var dto = new DailyComplaintDto { DailyComplaintId = 3, ConstituencyId = 8, IsCompleted = false };
        _mockService.Setup(s => s.GetDailyComplaintByLeaderIdAsync(77)).ReturnsAsync(dto);

        // Act
        var result = await _controller.GetDailyComplaintByLeaderId(77);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetDailyComplaintByLeaderId_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetDailyComplaintByLeaderIdAsync(55)).ReturnsAsync((DailyComplaintDto?)null);

        // Act
        var result = await _controller.GetDailyComplaintByLeaderId(55);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetDailyComplaintByConstituency_WithExisting_ReturnsOk()
    {
        // Arrange
        var dto = new DailyComplaintDto { DailyComplaintId = 10, ConstituencyId = 3, IsCompleted = true };
        _mockService.Setup(s => s.GetDailyComplaintByConstituencyNameAsync("Guntur")).ReturnsAsync(dto);

        // Act
        var result = await _controller.GetDailyComplaintByConstituency("Guntur");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetDailyComplaintByConstituency_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetDailyComplaintByConstituencyNameAsync("Vizag")).ReturnsAsync((DailyComplaintDto?)null);

        // Act
        var result = await _controller.GetDailyComplaintByConstituency("Vizag");

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No daily complaints found for constituency: Vizag", nf.Value);
    }

    private static DailyComplaintVm CreateVm(bool isCompleted = false, int constituencyId = 1)
        => new DailyComplaintVm { IsCompleted = isCompleted, ConstituencyId = constituencyId };
}
