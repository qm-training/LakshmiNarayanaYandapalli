namespace WelfareWorkTracker.Tests.Controllers;
public class EmailTemplateControllerTests
{
    private readonly Mock<IEmailTemplateService> _mockService;
    private readonly EmailTemplateController _controller;

    public EmailTemplateControllerTests()
    {
        _mockService = new Mock<IEmailTemplateService>();
        _controller = new EmailTemplateController(_mockService.Object);
    }

    [Fact]
    public async Task GetTemplateById_WithExisting_ReturnsOkWithDto()
    {
        // Arrange
        AttachUser(_controller);
        var dto = new EmailTemplate { Id = 5, Subject = "Sub", Body = "Body" };
        _mockService.Setup(s => s.GetTemplateByIdAsync(5)).ReturnsAsync(dto);

        // Act
        var result = await _controller.GetTemplateById(5);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetTemplateById_WhenServiceReturnsNull_ReturnsOkWithNull()
    {
        // Arrange
        AttachUser(_controller);
        _mockService.Setup(s => s.GetTemplateByIdAsync(404)).ReturnsAsync((EmailTemplate)null!);

        // Act
        var result = await _controller.GetTemplateById(404);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Null(ok.Value);
    }

    [Fact]
    public async Task GetAllTemplates_WithData_ReturnsOkWithList()
    {
        // Arrange
        AttachUser(_controller);
        var list = new List<EmailTemplate>
        {
            new EmailTemplate { Id = 1, Subject = "S1", Body = "B1" },
            new EmailTemplate { Id = 2, Subject = "S2", Body = "B2" }
        };
        _mockService.Setup(s => s.GetAllTemplatesAsync()).ReturnsAsync(list);

        // Act
        var result = await _controller.GetAllTemplates();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(list, ok.Value);
    }

    [Fact]
    public async Task GetAllTemplates_WhenEmpty_ReturnsOkWithEmptyList()
    {
        // Arrange
        AttachUser(_controller);
        var empty = new List<EmailTemplate>();
        _mockService.Setup(s => s.GetAllTemplatesAsync()).ReturnsAsync(empty);

        // Act
        var result = await _controller.GetAllTemplates();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(empty, ok.Value);
    }

    [Fact]
    public async Task GetAllTemplatesByTemplateName_WithMatches_ReturnsOkWithList()
    {
        // Arrange
        AttachUser(_controller);
        AttachUser(_controller);
var item = new EmailTemplate { Id = 10, Subject = "Welcome", Body = "Hi" };
_mockService
    .Setup(s => s.GetByNameAsync("Welcome"))
    .ReturnsAsync(item);

        // Act
        var result = await _controller.GetAllTemplatesByTemplateName("Welcome");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(item, ok.Value);
    }

    [Fact]
    public async Task GetAllTemplatesByTemplateName_WhenNoMatches_ReturnsOkWithNull()
    {
        // Arrange
        AttachUser(_controller);
        _mockService
            .Setup(s => s.GetByNameAsync("Unknown"))
            .ReturnsAsync((EmailTemplate)null!);

        // Act
        var result = await _controller.GetAllTemplatesByTemplateName("Unknown");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Null(ok.Value);
    }

    [Fact]
    public async Task AddTemplate_WithAdminRole_ReturnsCreated()
    {
        // Arrange
        var userId = 42;
        AttachUser(_controller, role: "Admin", userId: userId);
        var vm = CreateCreateVm("Subject", "Body");
        _mockService.Setup(s => s.AddTemplateAsync(userId, vm)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddTemplate(vm);

        // Assert
        var created = Assert.IsType<CreatedResult>(result);
        Assert.Equal(201, created.StatusCode);
        _mockService.Verify(s => s.AddTemplateAsync(userId, vm), Times.Once);
    }

    [Fact]
    public async Task AddTemplate_WithAdminRole_ServiceThrowsStillReturnsCreated_IfNotCaught()
    {
        // Arrange
        var userId = 7;
        AttachUser(_controller, role: "Admin", userId: userId);
        var vm = CreateCreateVm("S", "B");
        _mockService.Setup(s => s.AddTemplateAsync(userId, vm)).ThrowsAsync(new System.Exception("boom"));

        // Act & Assert
        await Assert.ThrowsAsync<System.Exception>(() => _controller.AddTemplate(vm));
    }

    [Fact]
    public async Task UpdateTemplate_WithAdminRole_ReturnsNoContent()
    {
        // Arrange
        var userId = 11;
        AttachUser(_controller, role: "Admin", userId: userId);
        var vm = new UpdateEmailTemplateVm { Subject = "New", Body = "Updated" };
        _mockService.Setup(s => s.UpdateTemplateAsync(5, userId, vm)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateTemplate(5, vm);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(s => s.UpdateTemplateAsync(5, userId, vm), Times.Once);
    }

    [Fact]
    public async Task UpdateTemplate_WithAdminRole_ServiceThrows_PropagatesException()
    {
        // Arrange
        var userId = 9;
        AttachUser(_controller, role: "Admin", userId: userId);
        var vm = new UpdateEmailTemplateVm { Subject = "X", Body = "Y" };
        _mockService.Setup(s => s.UpdateTemplateAsync(77, userId, vm)).ThrowsAsync(new System.Exception("update failed"));

        // Act & Assert
        await Assert.ThrowsAsync<System.Exception>(() => _controller.UpdateTemplate(77, vm));
    }

    [Fact]
    public async Task DeleteTemplate_WithAdminRole_ReturnsNoContent()
    {
        // Arrange
        var userId = 100;
        AttachUser(_controller, role: "Admin", userId: userId);
        _mockService.Setup(s => s.DeleteTemplateAsync(3, userId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteTemplate(3);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(s => s.DeleteTemplateAsync(3, userId), Times.Once);
    }

    [Fact]
    public async Task DeleteTemplate_WithAdminRole_ServiceThrows_PropagatesException()
    {
        // Arrange
        var userId = 101;
        AttachUser(_controller, role: "Admin", userId: userId);
        _mockService.Setup(s => s.DeleteTemplateAsync(9, userId)).ThrowsAsync(new System.Exception("delete failed"));

        // Act & Assert
        await Assert.ThrowsAsync<System.Exception>(() => _controller.DeleteTemplate(9));
    }

    private static void AttachUser(ControllerBase controller, string? role = null, int userId = 1)
    {
        var claims = new List<Claim> { new Claim("Id", userId.ToString()) };
        if (!string.IsNullOrWhiteSpace(role))
            claims.Add(new Claim(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    private static CreateEmailTemplateVm CreateCreateVm(string subject, string body)
        => new CreateEmailTemplateVm { Subject = subject, Body = body };
}
