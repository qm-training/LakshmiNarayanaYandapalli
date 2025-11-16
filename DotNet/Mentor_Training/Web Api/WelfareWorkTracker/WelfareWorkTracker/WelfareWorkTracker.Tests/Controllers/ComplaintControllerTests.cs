namespace WelfareWorkTracker.Tests.Controllers;
public class ComplaintControllerTests
{
    private readonly Mock<IComplaintService> _mockService;
    private readonly ComplaintController _controller;

    public ComplaintControllerTests()
    {
        _mockService = new Mock<IComplaintService>();
        _controller = new ComplaintController(_mockService.Object);
    }

    [Fact]
    public async Task AddComplaint_WithValidInput_ReturnsOkWithPayload()
    {
        // Arrange
        var vm = CreateComplaintVm();
        var dto = new ComplaintDto { ComplaintId = 101, Title = vm.Title, Description = vm.Description };
        _mockService.Setup(s => s.AddComplaintAsync(vm)).ReturnsAsync(dto);

        // Act
        var result = await _controller.AddComplaint(vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        var payload = ok.Value!;
        var msg = payload.GetType().GetProperty("messsage")!.GetValue(payload, null);
        var newComplaint = payload.GetType().GetProperty("newComplaint")!.GetValue(payload, null);
        Assert.Equal("Complaint added successfully", msg);
        Assert.Equal(dto, newComplaint);
    }

    [Fact]
    public async Task AddComplaint_WhenServiceReturnsNull_ReturnsOkWithNullNewComplaint()
    {
        // Arrange
        var vm = CreateComplaintVm();
        _mockService.Setup(s => s.AddComplaintAsync(vm)).ReturnsAsync((ComplaintDto?)null);

        // Act
        var result = await _controller.AddComplaint(vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = ok.Value!;
        var msg = payload.GetType().GetProperty("messsage")!.GetValue(payload, null);
        var newComplaint = payload.GetType().GetProperty("newComplaint")!.GetValue(payload, null);
        Assert.Equal("Complaint added successfully", msg);
        Assert.Null(newComplaint);
    }

    [Fact]
    public async Task GetComplaintByComplaintId_WithExistingId_ReturnsOk()
    {
        // Arrange
        var dto = new ComplaintDto { ComplaintId = 7, Title = "T", Description = "D" };
        _mockService.Setup(s => s.GetComplaintByComplaintIdAsync(7)).ReturnsAsync(dto);

        // Act
        var result = await _controller.GetComplaintByComplaintId(7);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetComplaintByComplaintId_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetComplaintByComplaintIdAsync(404)).ReturnsAsync((ComplaintDto?)null);

        // Act
        var result = await _controller.GetComplaintByComplaintId(404);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No Complaint found with complaintId: 404", nf.Value);
    }

    [Fact]
    public async Task UpdateComplaintByComplaintId_WithExistingComplaint_ReturnsOk()
    {
        // Arrange
        var vm = CreateComplaintVm("Updated");
        var dto = new ComplaintDto { ComplaintId = 9, Title = vm.Title, Description = vm.Description };
        _mockService.Setup(s => s.UpdateComplaintByComplaintIdAsync(9, vm)).ReturnsAsync(dto);

        // Act
        var result = await _controller.UpdateComplaintByComplaintId(9, vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task UpdateComplaintByComplaintId_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var vm = CreateComplaintVm();
        _mockService.Setup(s => s.UpdateComplaintByComplaintIdAsync(999, vm)).ReturnsAsync((ComplaintDto?)null);

        // Act
        var result = await _controller.UpdateComplaintByComplaintId(999, vm);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No Complaint found with complaintId: 999", nf.Value);
    }

    [Fact]
    public async Task DeleteComplaintByComplaintId_WithExistingComplaint_ReturnsOkWithMessageAndTrue()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteComplaintByComplaintIdAsync(5)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteComplaintByComplaintId(5);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = ok.Value!;
        var message = payload.GetType().GetProperty("message")!.GetValue(payload, null);
        var complaint = payload.GetType().GetProperty("complaint")!.GetValue(payload, null);
        Assert.Equal("Complaint deleted successfully", message);
        Assert.Equal(true, complaint);
    }

    [Fact]
    public async Task DeleteComplaintByComplaintId_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteComplaintByComplaintIdAsync(123)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteComplaintByComplaintId(123);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No Complaint found with complaintId: 123", nf.Value);
    }

    [Fact]
    public async Task GetComplaintsByUserId_WithValidUser_ReturnsOkWithList()
    {
        // Arrange
        var userId = 42;
        AttachUser(_controller, userId);
        var list = new List<ComplaintDto>
        {
            new ComplaintDto { ComplaintId = 1, Title = "A" },
            new ComplaintDto { ComplaintId = 2, Title = "B" }
        };
        _mockService.Setup(s => s.GetComplaintsByUserIdAsync(userId)).ReturnsAsync(list);

        // Act
        var result = await _controller.GetComplaintsByUserId();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(list, ok.Value);
    }

    [Fact]
    public async Task GetComplaintsByUserId_WhenEmpty_ReturnsOkWithEmptyList()
    {
        // Arrange
        var userId = 77;
        AttachUser(_controller, userId);
        var empty = new List<ComplaintDto>();
        _mockService.Setup(s => s.GetComplaintsByUserIdAsync(userId)).ReturnsAsync(empty);

        // Act
        var result = await _controller.GetComplaintsByUserId();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(empty, ok.Value);
    }

    [Fact]
    public async Task GetComplaintsByConstituencyName_WithData_ReturnsOkWithList()
    {
        // Arrange
        var list = new List<ComplaintDto> { new ComplaintDto { ComplaintId = 3, Title = "T" } };
        _mockService.Setup(s => s.GetComplaintsForAdminRepAsync("Guntur")).ReturnsAsync(list);

        // Act
        var result = await _controller.GetComplaintsByConstituencyName("Guntur");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(list, ok.Value);
    }

    [Fact]
    public async Task GetComplaintsByConstituencyName_WhenEmpty_ReturnsOkWithEmptyList()
    {
        // Arrange
        var empty = new List<ComplaintDto>();
        _mockService.Setup(s => s.GetComplaintsForAdminRepAsync("Vizag")).ReturnsAsync(empty);

        // Act
        var result = await _controller.GetComplaintsByConstituencyName("Vizag");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(empty, ok.Value);
    }

    [Fact]
    public async Task GetRecentComplaintsAsync_WithData_ReturnsOkWithList()
    {
        // Arrange
        var list = new List<ComplaintDto> { new ComplaintDto { ComplaintId = 90, Title = "Recent" } };
        _mockService.Setup(s => s.GetRecentComplaintsAsync("Hyderabad")).ReturnsAsync(list);

        // Act
        var result = await _controller.GetRecentComplaintsAsync("Hyderabad");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(list, ok.Value);
    }

    [Fact]
    public async Task GetRecentComplaintsAsync_WhenEmpty_ReturnsOkWithEmptyList()
    {
        // Arrange
        var empty = new List<ComplaintDto>();
        _mockService.Setup(s => s.GetRecentComplaintsAsync("Nellore")).ReturnsAsync(empty);

        // Act
        var result = await _controller.GetRecentComplaintsAsync("Nellore");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(empty, ok.Value);
    }

    private static ComplaintVm CreateComplaintVm(string title = "Title")
        => new ComplaintVm { Title = title, Description = "Desc", ConstituencyName = "Test" };

    private static void AttachUser(ControllerBase controller, int userId)
    {
        var claims = new List<Claim> { new Claim("Id", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }
}
