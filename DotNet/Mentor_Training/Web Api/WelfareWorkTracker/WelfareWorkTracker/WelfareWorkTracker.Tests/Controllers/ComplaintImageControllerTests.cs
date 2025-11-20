namespace WelfareWorkTracker.Tests.Controllers;
public class ComplaintImageControllerTests
{
    private readonly Mock<IComplaintImageService> _mockService;
    private readonly ComplaintImageController _controller;

    public ComplaintImageControllerTests()
    {
        _mockService = new Mock<IComplaintImageService>();
        _controller = new ComplaintImageController(_mockService.Object);
    }

    [Fact]
    public async Task AddComplaintImage_WithValidInput_ReturnsOkWithPayload()
    {
        // Arrange
        var vm = new ComplaintImageVm { ComplaintId = 10, ImageUrl = "img.jpg" };
        var dto = new ComplaintImageDto { ComplaintImageId = 1, ComplaintId = 10, ImageUrl = "img.jpg" };
        _mockService.Setup(s => s.AddComplaintImageAsync(vm)).ReturnsAsync(dto);

        // Act
        var result = await _controller.AddComplaintImage(vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        var payload = ok.Value!;
        var message = payload.GetType().GetProperty("message")!.GetValue(payload, null);
        var image = payload.GetType().GetProperty("image")!.GetValue(payload, null);
        Assert.Equal("Image added successfully", message);
        Assert.Equal(dto, image);
    }

    [Fact]
    public async Task AddComplaintImage_WhenComplaintNotFound_ReturnsNotFound()
    {
        // Arrange
        var vm = new ComplaintImageVm { ComplaintId = 999, ImageUrl = "x.png" };
        _mockService.Setup(s => s.AddComplaintImageAsync(vm)).ReturnsAsync((ComplaintImageDto?)null);

        // Act
        var result = await _controller.AddComplaintImage(vm);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"No such complaint exist with complaintId: {vm.ComplaintId}", nf.Value);
    }

    [Fact]
    public async Task GetComplaintById_WithAdminRole_ReturnsOk()
    {
        // Arrange
        AttachUser(_controller, role: "Admin");
        var dto = new ComplaintImageDto { ComplaintImageId = 7, ComplaintId = 1, ImageUrl = "a.jpg" };
        _mockService.Setup(s => s.GetComplaintImageByIdAsync(7)).ReturnsAsync(dto);

        // Act
        var result = await _controller.GetComplaintById(7);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetComplaintById_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        AttachUser(_controller, role: "Admin");
        _mockService.Setup(s => s.GetComplaintImageByIdAsync(404)).ReturnsAsync((ComplaintImageDto?)null);

        // Act
        var result = await _controller.GetComplaintById(404);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No ComplaintImage found with id: 404", nf.Value);
    }

    [Fact]
    public async Task GetComplaintByComplaintId_WithExisting_ReturnsOk()
    {
        // Arrange
        AttachUser(_controller);
        var dto = new ComplaintImageDto { ComplaintImageId = 3, ComplaintId = 20, ImageUrl = "c.png" };
        _mockService.Setup(s => s.GetComplaintImageByIdAsync(20)).ReturnsAsync(dto);

        // Act
        var result = await _controller.GetComplaintByComplaintId(20);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetComplaintByComplaintId_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        AttachUser(_controller);
        _mockService.Setup(s => s.GetComplaintImageByIdAsync(77)).ReturnsAsync((ComplaintImageDto?)null);

        // Act
        var result = await _controller.GetComplaintByComplaintId(77);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No ComplaintImage found with id: 77", nf.Value);
    }

    [Fact]
    public async Task GetAllComplaintImagesByComplaintId_WithImages_ReturnsOkWithList()
    {
        // Arrange
        var list = new List<ComplaintImageDto>
        {
            new ComplaintImageDto { ComplaintImageId = 1, ComplaintId = 5, ImageUrl = "1.jpg" },
            new ComplaintImageDto { ComplaintImageId = 2, ComplaintId = 5, ImageUrl = "2.jpg" }
        };
        _mockService.Setup(s => s.GetComplaintImagesByComplaintIdAsync(5)).ReturnsAsync(list);

        // Act
        var result = await _controller.GetAllComplaintImagesByComplaintId(5);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(list, ok.Value);
    }

    [Fact]
    public async Task GetAllComplaintImagesByComplaintId_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetComplaintImagesByComplaintIdAsync(66)).ReturnsAsync((List<ComplaintImageDto>?)null);

        // Act
        var result = await _controller.GetAllComplaintImagesByComplaintId(66);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No Images found with complaintId: 66", nf.Value);
    }

    [Fact]
    public async Task DeleteComplaintImagesById_WithExisting_ReturnsOkWithMessage()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteComplaintImageByIdAsync(9)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteComplaintImagesById(9);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Image deleted successfully", ok.Value);
    }

    [Fact]
    public async Task DeleteComplaintImagesById_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteComplaintImageByIdAsync(123)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteComplaintImagesById(123);

        // Assert
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No image found", nf.Value);
    }

    private static void AttachUser(ControllerBase controller, string? role = null)
    {
        var claims = new List<Claim> { new Claim("Id", "1") };
        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }
}
