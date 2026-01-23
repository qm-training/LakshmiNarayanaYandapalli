namespace WelfareWorkTracker.Tests.Controllers;
public class ComplaintStatusControllerTests
{
    private readonly Mock<IComplaintStatusService> _mockService;
    private readonly ComplaintStatusController _controller;

    public ComplaintStatusControllerTests()
    {
        _mockService = new Mock<IComplaintStatusService>();
        _controller = new ComplaintStatusController(_mockService.Object);
    }

    [Fact]
    public async Task GetComplaintStatusHistory_WithNonEmptyHistory_ReturnsOk()
    {
        // Arrange
        var list = new List<ComplaintStatusDto>
            {
                new ComplaintStatusDto { ComplaintStatusId = 1, ComplaintId = 10, Status = 1 }
            };
        _mockService.Setup(s => s.GetComplaintStatusHistoryAsync(10)).ReturnsAsync(list);

        // Act
        var result = await _controller.GetComplaintStatusHistory(10);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(list, ok.Value);
    }

    [Fact]
    public async Task GetComplaintStatusHistory_WhenEmpty_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetComplaintStatusHistoryAsync(11)).ReturnsAsync(new List<ComplaintStatusDto>());

        // Act
        var result = await _controller.GetComplaintStatusHistory(11);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No status history found for the specified complaint.", nf.Value);
    }

    [Fact]
    public async Task GetComplaintStatusByComplaintId_WithExisting_ReturnsOk()
    {
        // Arrange
        var dto = new ComplaintStatusDto { ComplaintStatusId = 2, ComplaintId = 20, Status = 2 };
        _mockService.Setup(s => s.GetComplaintStatusByComplaintId(20)).ReturnsAsync(dto);

        // Act
        var result = await _controller.GetComplaintStatusByComplaintId(20);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetComplaintStatusByComplaintId_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetComplaintStatusByComplaintId(404)).ReturnsAsync((ComplaintStatusDto?)null);

        // Act
        var result = await _controller.GetComplaintStatusByComplaintId(404);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(" No complaint found with complaintId 404", nf.Value);
    }

    [Fact]
    public async Task GetComplaintStatusById_WithAdminRole_ReturnsOk()
    {
        // Arrange
        AttachUser(_controller, "Admin");
        var dto = new ComplaintStatusDto { ComplaintStatusId = 7, ComplaintId = 70, Status = 3 };
        _mockService.Setup(s => s.GetComplaintStatusByIdAsync(7)).ReturnsAsync(dto);

        // Act
        var result = await _controller.GetComplaintStatusById(7);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetComplaintStatusById_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        AttachUser(_controller, "Admin");
        _mockService.Setup(s => s.GetComplaintStatusByIdAsync(808)).ReturnsAsync((ComplaintStatusDto?)null);

        // Act
        var result = await _controller.GetComplaintStatusById(808);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(" No complaint found with complaintId 808", nf.Value);
    }

    [Fact]
    public async Task UpdateComplaintStatusByLeader_WithExistingComplaint_ReturnsOk()
    {
        // Arrange
        AttachUser(_controller, "Leader");
        var vm = new StatusByLeaderVm
        {
            ComplaintId = 33,
            Status = 2,
            DeadlineDate = System.DateTime.UtcNow
        };
        var dto = new ComplaintStatusDto { ComplaintStatusId = 100, ComplaintId = 33, Status = 2 };
        _mockService.Setup(s => s.UpdateComplaintStatusByLeaderAsync(vm)).ReturnsAsync(dto);

        // Act
        var result = await _controller.UpdateComplaintStatusByLeader(vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task UpdateComplaintStatusByLeader_WhenComplaintNotFound_ReturnsNotFound()
    {
        // Arrange
        AttachUser(_controller, "Leader");
        var vm = new StatusByLeaderVm
        {
            ComplaintId = 909,
            Status = 3,
            DeadlineDate = System.DateTime.UtcNow
        };
        _mockService.Setup(s => s.UpdateComplaintStatusByLeaderAsync(vm)).ReturnsAsync((ComplaintStatusDto?)null);

        // Act
        var result = await _controller.UpdateComplaintStatusByLeader(vm);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No complaint found with complaintId 909", nf.Value);
    }

    [Fact]
    public async Task VerifyComplaintByAdminRep_WithExistingComplaint_ReturnsOk()
    {
        // Arrange
        AttachUser(_controller, "AdminRepresentative");
        var vm = new StatusByAdminRepVm
        {
            ComplaintId = 55,
            Status = 1,
            ReferenceNumber = 0,
            ExpectedDeadline = null,
            MaxExtendableDeadline = null,
            RejectReason = null
        };
        var dto = new ComplaintStatusDto { ComplaintStatusId = 501, ComplaintId = 55, Status = 1 };
        _mockService.Setup(s => s.AddComplaintStatusByAdminRepAsync(vm)).ReturnsAsync(dto);

        // Act
        var result = await _controller.VerifyComplaintByAdminRep(vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task VerifyComplaintByAdminRep_WhenComplaintNotFound_ReturnsNotFound()
    {
        // Arrange
        AttachUser(_controller, "AdminRepresentative");
        var vm = new StatusByAdminRepVm
        {
            ComplaintId = 404,
            Status = 0
        };
        _mockService.Setup(s => s.AddComplaintStatusByAdminRepAsync(vm)).ReturnsAsync((ComplaintStatusDto?)null);

        // Act
        var result = await _controller.VerifyComplaintByAdminRep(vm);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Complaint does not exists with complaintId:404", nf.Value);
    }

    [Fact]
    public async Task VerifyComplaintByAdmin_WithExistingComplaint_ReturnsOk()
    {
        // Arrange
        AttachUser(_controller, "Admin");
        var vm = new StatusByAdminVm
        {
            ComplaintId = 77,
            Status = 2
        };
        var dto = new ComplaintStatusDto { ComplaintStatusId = 701, ComplaintId = 77, Status = 2 };
        _mockService.Setup(s => s.AddComplaintStatusByAdminAsync(vm)).ReturnsAsync(dto);

        // Act
        var result = await _controller.VerifyComplaintByAdmin(vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task VerifyComplaintByAdmin_WhenComplaintNotFound_ReturnsNotFound()
    {
        // Arrange
        AttachUser(_controller, "Admin");
        var vm = new StatusByAdminVm
        {
            ComplaintId = 888,
            Status = 0
        };
        _mockService.Setup(s => s.AddComplaintStatusByAdminAsync(vm)).ReturnsAsync((ComplaintStatusDto?)null);

        // Act
        var result = await _controller.VerifyComplaintByAdmin(vm);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Complaint does not exists with complaintId:888", nf.Value);
    }

    private static void AttachUser(ControllerBase controller, string role)
    {
        var claims = new List<Claim>
            {
                new Claim("Id", "1"),
                new Claim(ClaimTypes.Role, role)
            };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }
}