namespace WelfareWorkTracker.Tests.Infrastructure.Services;
public class ComplaintServiceTests
{
    private readonly Mock<IComplaintRepository> _mockComplaintRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IComplaintImageRepository> _mockComplaintImageRepository;
    private readonly Mock<IComplaintStatusRepository> _mockComplaintStatusRepository;
    private readonly Mock<IConstituencyRepository> _mockConstituencyRepository;
    private readonly Mock<IClaimsService> _mockClaimsService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IEmailTemplateRepository> _mockEmailTemplateRepository;
    private readonly ComplaintService _complaintService;

    public ComplaintServiceTests()
    {
        _mockComplaintRepository = new Mock<IComplaintRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockComplaintImageRepository = new Mock<IComplaintImageRepository>();
        _mockComplaintStatusRepository = new Mock<IComplaintStatusRepository>();
        _mockConstituencyRepository = new Mock<IConstituencyRepository>();
        _mockClaimsService = new Mock<IClaimsService>();
        _mockMapper = new Mock<IMapper>();
        _mockEmailService = new Mock<IEmailService>();
        _mockEmailTemplateRepository = new Mock<IEmailTemplateRepository>();

        _complaintService = new ComplaintService(
            _mockComplaintRepository.Object,
            _mockUserRepository.Object,
            _mockComplaintImageRepository.Object,
            _mockComplaintStatusRepository.Object,
            _mockConstituencyRepository.Object,
            _mockClaimsService.Object,
            _mockMapper.Object,
            _mockEmailService.Object,
            _mockEmailTemplateRepository.Object);
    }

    [Fact]
    public async Task AddComplaintAsync_WithValidInput_ReturnsComplaintDtoWithImagesAndStatus()
    {
        // Arrange
        var complaintVm = CreateComplaintVm(images: new List<string> { "url1", "url2" });
        var constituencyId = 100;
        var userId = 5;
        var leader = CreateUser(userId: 20, constituencyId, "leader@test.com");
        var citizen = CreateUser(userId, constituencyId, "citizen@test.com");
        var adminRep = CreateUser(userId: 30, constituencyId, "adminrep@test.com");

        var addedComplaint = CreateComplaint(complaintId: 1, constituencyId, citizenId: userId, leaderId: leader.UserId);
        var addedStatus = CreateComplaintStatus(1, (int)Status.UnderValidation);
        var complaintDto = CreateComplaintDto(1, complaintVm.Title, complaintVm.Description);

        var citizenTemplate = CreateEmailTemplate(10, Constants.EmailTemplates.NewComplaintCitizen);
        var adminRepTemplate = CreateEmailTemplate(11, Constants.EmailTemplates.NewComplaintAdminRep);

        _mockConstituencyRepository
            .Setup(r => r.GetConstituencyIdByNameAsync(complaintVm.ConstituencyName))
            .ReturnsAsync(constituencyId);

        _mockUserRepository
            .Setup(r => r.GetLeaderByConstituencyIdAsync(constituencyId))
            .ReturnsAsync(leader);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockComplaintRepository
            .Setup(r => r.AddComplaintAsync(It.IsAny<Complaint>()))
            .ReturnsAsync((Complaint c) =>
            {
                c.ComplaintId = addedComplaint.ComplaintId;
                return c;
            });

        _mockComplaintImageRepository
            .Setup(r => r.AddComplaintImageAsync(It.IsAny<ComplaintImage>()))
            .ReturnsAsync((ComplaintImage ci) => ci);

        _mockComplaintStatusRepository
            .Setup(r => r.AddComplaintStatusAsync(It.IsAny<ComplaintStatus>()))
            .ReturnsAsync(addedStatus);

        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(userId))
            .ReturnsAsync(citizen);

        _mockEmailTemplateRepository
            .Setup(r => r.GetByNameAsync(Constants.EmailTemplates.NewComplaintCitizen))
            .ReturnsAsync(citizenTemplate);

        _mockUserRepository
            .Setup(r => r.GetAdminRepByConstituencyIdAsync(constituencyId))
            .ReturnsAsync(adminRep);

        _mockEmailTemplateRepository
            .Setup(r => r.GetByNameAsync(Constants.EmailTemplates.NewComplaintAdminRep))
            .ReturnsAsync(adminRepTemplate);

        _mockEmailService
            .Setup(s => s.SendEmailAsync(It.IsAny<EmailVm>()))
            .ReturnsAsync(true);

        _mockMapper
            .Setup(m => m.Map<ComplaintDto>(It.IsAny<Complaint>()))
            .Returns(complaintDto);

        // Act
        var result = await _complaintService.AddComplaintAsync(complaintVm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(addedComplaint.ComplaintId, result.ComplaintId);
        Assert.Equal(complaintVm.Title, result.Title);
        Assert.Equal(2, result.Images.Count);
        Assert.Equal(Status.UnderValidation.ToString(), result.Status);

        _mockComplaintRepository.Verify(r => r.AddComplaintAsync(It.IsAny<Complaint>()), Times.Once);
        _mockComplaintImageRepository.Verify(r => r.AddComplaintImageAsync(It.IsAny<ComplaintImage>()), Times.Exactly(2));
        _mockComplaintStatusRepository.Verify(r => r.AddComplaintStatusAsync(It.IsAny<ComplaintStatus>()), Times.Once);
        _mockEmailService.Verify(s => s.SendEmailAsync(It.IsAny<EmailVm>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddComplaintAsync_WithoutImages_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var complaintVm = CreateComplaintVm(images: new List<string>());
        var constituencyId = 100;
        var leader = CreateUser(userId: 20, constituencyId, "leader@test.com");
        var userId = 5;

        _mockConstituencyRepository
            .Setup(r => r.GetConstituencyIdByNameAsync(complaintVm.ConstituencyName))
            .ReturnsAsync(constituencyId);

        _mockUserRepository
            .Setup(r => r.GetLeaderByConstituencyIdAsync(constituencyId))
            .ReturnsAsync(leader);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintService.AddComplaintAsync(complaintVm));

        // Assert
        Assert.Equal("Atleast one image is required", exception.Message);
        _mockComplaintRepository.Verify(r => r.AddComplaintAsync(It.IsAny<Complaint>()), Times.Never);
    }

    [Fact]
    public async Task DeleteComplaintByComplaintIdAsync_WithValidUserAndUnderValidationStatus_ReturnsTrue()
    {
        // Arrange
        var complaintId = 1;
        var userId = 5;

        var complaint = CreateComplaint(complaintId, constituencyId: 100, citizenId: userId, leaderId: 20);
        var status = CreateComplaintStatus(complaintId, status: 1);
        var images = new List<ComplaintImage>
        {
            CreateComplaintImage(1, complaintId, "url1"),
            CreateComplaintImage(2, complaintId, "url2")
        };

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(
                It.Is<int?>(id => id == complaintId),
                It.IsAny<int?>()))
            .ReturnsAsync(status);

        _mockComplaintStatusRepository
            .Setup(r => r.DeleteComplaintStatusByComplaintIdAsync(complaintId))
            .ReturnsAsync(true);

        _mockComplaintImageRepository
            .Setup(r => r.GetAllComplaintImagesByComplaintIdAsync(complaintId))
            .ReturnsAsync(images);

        _mockComplaintImageRepository
            .Setup(r => r.RemoveComplaintImageAsync(It.IsAny<ComplaintImage>()))
            .Returns(Task.CompletedTask);

        _mockComplaintRepository
            .Setup(r => r.DeleteComplaintByComplaintAsync(complaint))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _complaintService.DeleteComplaintByComplaintIdAsync(complaintId);

        // Assert
        Assert.True(result);

        _mockComplaintStatusRepository.Verify(
            r => r.GetComplaintStatusAsync(It.Is<int?>(id => id == complaintId), It.IsAny<int?>()),
            Times.Once);

        _mockComplaintImageRepository.Verify(r => r.RemoveComplaintImageAsync(It.IsAny<ComplaintImage>()), Times.Exactly(2));
        _mockComplaintRepository.Verify(r => r.DeleteComplaintByComplaintAsync(complaint), Times.Once);
    }

    [Fact]
    public async Task DeleteComplaintByComplaintIdAsync_WhenComplaintNotFound_ReturnsFalse()
    {
        // Arrange
        var complaintId = 1;

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync((Complaint?)null);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(5);

        // Act
        var result = await _complaintService.DeleteComplaintByComplaintIdAsync(complaintId);

        // Assert
        Assert.False(result);

        _mockComplaintStatusRepository.Verify(
            r => r.GetComplaintStatusAsync(It.IsAny<int?>(), It.IsAny<int?>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteComplaintByComplaintIdAsync_WhenUserIsNotOwner_ThrowsUnauthorized()
    {
        // Arrange
        var complaintId = 1;
        var userId = 5;
        var otherUserId = 99;

        var complaint = CreateComplaint(complaintId, constituencyId: 100, citizenId: otherUserId, leaderId: 20);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintService.DeleteComplaintByComplaintIdAsync(complaintId));

        // Assert
        Assert.Equal("you cannot delete", exception.Message);
        Assert.Equal((int)HttpStatusCode.Unauthorized, exception.StatusCode);
        _mockComplaintStatusRepository.Verify(
            r => r.GetComplaintStatusAsync(It.IsAny<int?>(), It.IsAny<int?>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteComplaintByComplaintIdAsync_WhenStatusIsNotUnderValidation_ThrowsException()
    {
        // Arrange
        var complaintId = 1;
        var userId = 5;

        var complaint = CreateComplaint(complaintId, constituencyId: 100, citizenId: userId, leaderId: 20);
        var status = CreateComplaintStatus(complaintId, status: 2);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(
                It.Is<int?>(id => id == complaintId),
                It.IsAny<int?>()))
            .ReturnsAsync(status);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintService.DeleteComplaintByComplaintIdAsync(complaintId));

        // Assert
        Assert.Equal("you cannot delete", exception.Message);
        _mockComplaintRepository.Verify(r => r.DeleteComplaintByComplaintAsync(It.IsAny<Complaint>()), Times.Never);
    }

    [Fact]
    public async Task GetComplaintByComplaintIdAsync_WithExistingComplaint_ReturnsDtoWithImagesAndStatus()
    {
        // Arrange
        var complaintId = 1;
        var complaint = CreateComplaint(complaintId, constituencyId: 100, citizenId: 5, leaderId: 20);
        var images = new List<ComplaintImage>
        {
            CreateComplaintImage(1, complaintId, "url1"),
            CreateComplaintImage(2, complaintId, "url2")
        };
        var status = CreateComplaintStatus(complaintId, (int)Status.UnderValidation);
        var dto = CreateComplaintDto(complaintId, complaint.Title, complaint.Description);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockComplaintImageRepository
            .Setup(r => r.GetAllComplaintImagesByComplaintIdAsync(complaintId))
            .ReturnsAsync(images);

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(
                It.Is<int?>(id => id == complaintId),
                It.IsAny<int?>()))
            .ReturnsAsync(status);

        _mockMapper
            .Setup(m => m.Map<ComplaintDto>(complaint))
            .Returns(dto);

        // Act
        var result = await _complaintService.GetComplaintByComplaintIdAsync(complaintId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(complaintId, result!.ComplaintId);
        Assert.Equal(2, result.Images.Count);
        Assert.Equal(Status.UnderValidation.ToString(), result.Status);
    }

    [Fact]
    public async Task GetComplaintByComplaintIdAsync_WhenComplaintIsNull_ReturnsNull()
    {
        // Arrange
        var complaintId = 1;

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync((Complaint?)null);

        _mockComplaintImageRepository
            .Setup(r => r.GetAllComplaintImagesByComplaintIdAsync(complaintId))
            .ReturnsAsync(new List<ComplaintImage>());

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(
                It.Is<int?>(id => id == complaintId),
                It.IsAny<int?>()))
            .ReturnsAsync((ComplaintStatus?)null);

        // Act
        var result = await _complaintService.GetComplaintByComplaintIdAsync(complaintId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetComplaintsForLeaderByLeaderIdAsync_WithMatchingStatus_ReturnsFilteredDtos()
    {
        // Arrange
        var leaderId = 20;
        var complaint1 = CreateComplaint(1, 100, 5, leaderId);
        var complaint2 = CreateComplaint(2, 100, 6, leaderId);

        var complaints = new List<Complaint> { complaint1, complaint2 };

        var status1 = CreateComplaintStatus(1, (int)Status.Backlog);
        var status2 = CreateComplaintStatus(2, (int)Status.UnderValidation);

        var images1 = new List<ComplaintImage> { CreateComplaintImage(1, 1, "url1") };
        var images2 = new List<ComplaintImage> { CreateComplaintImage(2, 2, "url2") };

        var dto1 = CreateComplaintDto(1, complaint1.Title, complaint1.Description);
        var dto2 = CreateComplaintDto(2, complaint2.Title, complaint2.Description);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintsForLeaderByLeaderIdAsync(leaderId))
            .ReturnsAsync(complaints);

        _mockComplaintStatusRepository
            .SetupSequence(r => r.GetComplaintStatusAsync(It.IsAny<int?>(), It.IsAny<int?>()))
            .ReturnsAsync(status1)
            .ReturnsAsync(status2);

        _mockComplaintImageRepository
            .Setup(r => r.GetAllComplaintImagesByComplaintIdAsync(1))
            .ReturnsAsync(images1);

        _mockComplaintImageRepository
            .Setup(r => r.GetAllComplaintImagesByComplaintIdAsync(2))
            .ReturnsAsync(images2);

        _mockMapper
            .Setup(m => m.Map<ComplaintDto>(complaint1))
            .Returns(dto1);

        _mockMapper
            .Setup(m => m.Map<ComplaintDto>(complaint2))
            .Returns(dto2);

        // Act
        var result = await _complaintService.GetComplaintsForLeaderByLeaderIdAsync(leaderId, Status.UnderValidation);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result!);
        Assert.Equal(2, result[0].ComplaintId);
        Assert.Equal("url2", result[0].Images.Single());
        Assert.Equal(Status.UnderValidation.ToString(), result[0].Status);
    }

    [Fact]
    public async Task GetComplaintsForLeaderByLeaderIdAsync_WhenComplaintsNull_ReturnsNull()
    {
        // Arrange
        var leaderId = 20;

        _mockComplaintRepository
            .Setup(r => r.GetComplaintsForLeaderByLeaderIdAsync(leaderId))
            .ReturnsAsync((List<Complaint>)null!);

        // Act
        var result = await _complaintService.GetComplaintsForLeaderByLeaderIdAsync(leaderId, Status.UnderValidation);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateComplaintByComplaintIdAsync_WithValidUserAndUnderValidation_UpdatesAndReturnsDto()
    {
        // Arrange
        var complaintId = 1;
        var userId = 5;
        var constituencyId = 100;
        var leader = CreateUser(userId: 20, constituencyId, "leader@test.com");

        var complaint = CreateComplaint(complaintId, constituencyId, citizenId: userId, leaderId: leader.UserId);
        var status = CreateComplaintStatus(complaintId, (int)Status.UnderValidation);
        var existingImages = new List<ComplaintImage>
        {
            CreateComplaintImage(1, complaintId, "old1"),
            CreateComplaintImage(2, complaintId, "old2")
        };
        var newImageUrls = new List<string> { "new1", "new2" };
        var updatedImages = new List<ComplaintImage>
        {
            CreateComplaintImage(3, complaintId, "new1"),
            CreateComplaintImage(4, complaintId, "new2")
        };
        var complaintVm = CreateComplaintVm(images: newImageUrls);
        var dto = CreateComplaintDto(complaintId, complaintVm.Title, complaintVm.Description);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(
                It.Is<int?>(id => id == complaintId),
                It.IsAny<int?>()))
            .ReturnsAsync(status);

        _mockUserRepository
            .Setup(r => r.GetLeaderByConstituencyIdAsync(constituencyId))
            .ReturnsAsync(leader);

        _mockComplaintImageRepository
            .Setup(r => r.GetAllComplaintImagesByComplaintIdAsync(complaintId))
            .ReturnsAsync(existingImages);

        _mockComplaintImageRepository
            .Setup(r => r.RemoveComplaintImageAsync(It.IsAny<ComplaintImage>()))
            .Returns(Task.CompletedTask);

        _mockComplaintImageRepository
            .Setup(r => r.AddComplaintImageAsync(It.IsAny<ComplaintImage>()))
            .ReturnsAsync((ComplaintImage ci) => ci);

        _mockComplaintImageRepository
            .SetupSequence(r => r.GetAllComplaintImagesByComplaintIdAsync(complaintId))
            .ReturnsAsync(existingImages)
            .ReturnsAsync(updatedImages);

        _mockComplaintRepository
            .Setup(r => r.UpdateComplaintByIdAsync(complaint))
            .Returns(Task.CompletedTask);

        _mockMapper
            .Setup(m => m.Map<ComplaintDto>(complaint))
            .Returns(dto);

        // Act
        var result = await _complaintService.UpdateComplaintByComplaintIdAsync(complaintId, complaintVm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(complaintId, result!.ComplaintId);
        Assert.Equal(complaintVm.Title, result.Title);
        Assert.Equal(2, result.Images.Count);
        Assert.Equal(Status.UnderValidation.ToString(), result.Status);
    }

    [Fact]
    public async Task UpdateComplaintByComplaintIdAsync_WhenComplaintNotFound_ReturnsNull()
    {
        // Arrange
        var complaintId = 1;
        var complaintVm = CreateComplaintVm(images: new List<string> { "url1" });

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync((Complaint?)null);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(5);

        // Act
        var result = await _complaintService.UpdateComplaintByComplaintIdAsync(complaintId, complaintVm);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateComplaintByComplaintIdAsync_WhenUserIsNotOwner_ThrowsUnauthorized()
    {
        // Arrange
        var complaintId = 1;
        var complaintVm = CreateComplaintVm(images: new List<string> { "url1" });
        var complaint = CreateComplaint(complaintId, constituencyId: 100, citizenId: 999, leaderId: 20);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(5);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintService.UpdateComplaintByComplaintIdAsync(complaintId, complaintVm));

        // Assert
        Assert.Equal("You cannot update this complaint", exception.Message);
        Assert.Equal((int)HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task UpdateComplaintByComplaintIdAsync_WhenStatusIsNotUnderValidation_ThrowsException()
    {
        // Arrange
        var complaintId = 1;
        var complaintVm = CreateComplaintVm(images: new List<string> { "url1" });
        var complaint = CreateComplaint(complaintId, constituencyId: 100, citizenId: 5, leaderId: 20);
        var status = CreateComplaintStatus(complaintId, (int)Status.Backlog);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(5);

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(
                It.Is<int?>(id => id == complaintId),
                It.IsAny<int?>()))
            .ReturnsAsync(status);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintService.UpdateComplaintByComplaintIdAsync(complaintId, complaintVm));

        // Assert
        Assert.Equal("This complaint cannot be edited at this stage", exception.Message);
    }

    private static ComplaintVm CreateComplaintVm(List<string> images)
    {
        return new ComplaintVm
        {
            Title = "Test Complaint",
            Description = "Test Description",
            ConstituencyName = "Test Constituency",
            Images = images
        };
    }

    private static Complaint CreateComplaint(int complaintId, int constituencyId, int citizenId, int leaderId)
    {
        return new Complaint
        {
            ComplaintId = complaintId,
            Title = "Test Complaint",
            Description = "Test Description",
            ConstituencyName = "Test Constituency",
            ConstituencyId = constituencyId,
            CitizenId = citizenId,
            LeaderId = leaderId,
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
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };
    }

    private static ComplaintImage CreateComplaintImage(int complaintImageId, int complaintId, string url)
    {
        return new ComplaintImage
        {
            ComplaintImageId = complaintImageId,
            ComplaintId = complaintId,
            ImageUrl = url,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };
    }

    private static ComplaintDto CreateComplaintDto(int complaintId, string title, string description)
    {
        return new ComplaintDto
        {
            ComplaintId = complaintId,
            Title = title,
            Description = description,
            Images = new List<string>()
        };
    }

    private static User CreateUser(int userId, int constituencyId, string email)
    {
        return new User
        {
            UserId = userId,
            ConstituencyId = constituencyId,
            Email = email,
            FullName = $"User {userId}",
            Reputation = 50
        };
    }

    private static EmailTemplate CreateEmailTemplate(int id, string name)
    {
        return new EmailTemplate
        {
            Id = id,
            Name = name,
            Body = "Body",
            Subject = "Subject",
            IsActive = true,
            DateCreated = DateTime.UtcNow
        };
    }
}
