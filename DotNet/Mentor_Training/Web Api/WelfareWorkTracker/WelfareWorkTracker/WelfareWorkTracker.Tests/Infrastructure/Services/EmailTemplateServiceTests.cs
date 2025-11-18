namespace WelfareWorkTracker.Tests.Infrastructure.Services;
public class EmailTemplateServiceTests
{
    private readonly Mock<IEmailTemplateRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly EmailTemplateService _service;

    public EmailTemplateServiceTests()
    {
        _mockRepo = new Mock<IEmailTemplateRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new EmailTemplateService(_mockRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetTemplateByIdAsync_WithValidId_ReturnsTemplate()
    {
        // Arrange
        var tpl = CreateTemplate(10, createdBy: 1);
        _mockRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(tpl);

        // Act
        var result = await _service.GetTemplateByIdAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        _mockRepo.Verify(r => r.GetByIdAsync(10), Times.Once);
    }

    [Fact]
    public async Task GetAllTemplatesAsync_WithData_ReturnsList()
    {
        // Arrange
        var list = new List<EmailTemplate> { CreateTemplate(1, 1), CreateTemplate(2, 1) };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

        // Act
        var result = await _service.GetAllTemplatesAsync();

        // Assert
        Assert.Equal(2, result.Count);
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }
    [Fact]
    public async Task AddTemplateAsync_WithValidInput_AddsTemplate()
    {
        // Arrange
        var vm = new CreateEmailTemplateVm { Name = "Welcome", Subject = "Hi", Body = "Hello" };

        var mapped = new EmailTemplate
        {
            Name = vm.Name,
            Subject = vm.Subject,
            Body = vm.Body
        };

        _mockMapper
            .Setup(m => m.Map<EmailTemplate>(vm))
            .Returns(mapped);

        _mockRepo
            .Setup(r => r.AddAsync(It.IsAny<EmailTemplate>()))
            .ReturnsAsync(true);

        // Act
        await _service.AddTemplateAsync(7, vm);

        // Assert
        _mockMapper.Verify(m => m.Map<EmailTemplate>(vm), Times.Once);

        _mockRepo.Verify(r => r.AddAsync(It.Is<EmailTemplate>(e =>
            ReferenceEquals(e, mapped) &&
            e.CreatedBy == 7 &&
            e.Name == vm.Name &&
            e.Subject == vm.Subject &&
            e.Body == vm.Body &&
            e.DateCreated != default
        )), Times.Once);
    }


    [Fact]
    public async Task UpdateTemplateAsync_WithValidInput_UpdatesTemplate()
    {
        // Arrange
        var existing = CreateTemplate(22, createdBy: 9);
        var vm = new UpdateEmailTemplateVm { Subject = "NewSub", Body = "NewBody" };

        _mockRepo.Setup(r => r.GetByIdAsync(22)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<EmailTemplate>())).ReturnsAsync(true);

        // Act
        await _service.UpdateTemplateAsync(22, 9, vm);

        // Assert
        Assert.Equal("NewSub", existing.Subject);
        Assert.Equal("NewBody", existing.Body);
        _mockRepo.Verify(r => r.UpdateAsync(It.Is<EmailTemplate>(e =>
            e.Id == 22 && e.Subject == "NewSub" && e.Body == "NewBody" && e.DateUpdated != default
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateTemplateAsync_WhenTemplateNotFound_Throws()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(55)).ReturnsAsync((EmailTemplate)null!);

        // Act
        var act = () => _service.UpdateTemplateAsync(55, 1, new UpdateEmailTemplateVm());

        // Assert
        await Assert.ThrowsAsync<WelfareWorkTrackerException>(act);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<EmailTemplate>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTemplateAsync_WhenUserNotCreator_Throws()
    {
        // Arrange
        var existing = CreateTemplate(22, createdBy: 99);
        _mockRepo.Setup(r => r.GetByIdAsync(22)).ReturnsAsync(existing);

        // Act
        var act = () => _service.UpdateTemplateAsync(22, 9, new UpdateEmailTemplateVm { Subject = "S", Body = "B" });

        // Assert
        await Assert.ThrowsAsync<WelfareWorkTrackerException>(act);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<EmailTemplate>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTemplateAsync_WithValidInput_SoftDeletesTemplate()
    {
        // Arrange
        var existing = CreateTemplate(33, createdBy: 4);
        _mockRepo.Setup(r => r.GetByIdAsync(33)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<EmailTemplate>())).ReturnsAsync(true);

        // Act
        await _service.DeleteTemplateAsync(33, 4);

        // Assert
        Assert.False(existing.IsActive);
        _mockRepo.Verify(r => r.UpdateAsync(It.Is<EmailTemplate>(e =>
            e.Id == 33 && !e.IsActive && e.DateUpdated != default
        )), Times.Once);
    }

    [Fact]
    public async Task DeleteTemplateAsync_WhenTemplateNotFound_Throws()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(44)).ReturnsAsync((EmailTemplate)null!);

        // Act
        var act = () => _service.DeleteTemplateAsync(44, 1);

        // Assert
        await Assert.ThrowsAsync<WelfareWorkTrackerException>(act);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<EmailTemplate>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTemplateAsync_WhenUserNotCreator_Throws()
    {
        // Arrange
        var existing = CreateTemplate(33, createdBy: 10);
        _mockRepo.Setup(r => r.GetByIdAsync(33)).ReturnsAsync(existing);

        // Act
        var act = () => _service.DeleteTemplateAsync(33, 9);

        // Assert
        await Assert.ThrowsAsync<WelfareWorkTrackerException>(act);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<EmailTemplate>()), Times.Never);
    }

    [Fact]
    public async Task GetByNameAsync_WithName_ReturnsTemplate()
    {
        // Arrange
        var tpl = CreateTemplate(5, createdBy: 2, name: "Welcome");
        _mockRepo.Setup(r => r.GetByNameAsync("Welcome")).ReturnsAsync(tpl);

        // Act
        var result = await _service.GetByNameAsync("Welcome");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        _mockRepo.Verify(r => r.GetByNameAsync("Welcome"), Times.Once);
    }
    private static EmailTemplate CreateTemplate(int id, int createdBy, string name = "Tpl") =>
        new EmailTemplate
        {
            Id = id,
            Name = name,
            Subject = "Sub",
            Body = "Body",
            CreatedBy = createdBy,
            IsActive = true
        };
}