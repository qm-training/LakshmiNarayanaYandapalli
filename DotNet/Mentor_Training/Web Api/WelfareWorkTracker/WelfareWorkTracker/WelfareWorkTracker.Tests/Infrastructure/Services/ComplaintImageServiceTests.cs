namespace WelfareWorkTracker.Tests.Infrastructure.Services;
public class ComplaintImageServiceTests
{
    private readonly Mock<IComplaintImageRepository> _mockComplaintImageRepository;
    private readonly Mock<IComplaintRepository> _mockComplaintRepository;
    private readonly Mock<IClaimsService> _mockClaimsService;
    private readonly Mock<IComplaintStatusRepository> _mockComplaintStatusRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ComplaintImageService _complaintImageService;

    public ComplaintImageServiceTests()
    {
        _mockComplaintImageRepository = new Mock<IComplaintImageRepository>();
        _mockComplaintRepository = new Mock<IComplaintRepository>();
        _mockClaimsService = new Mock<IClaimsService>();
        _mockComplaintStatusRepository = new Mock<IComplaintStatusRepository>();
        _mockMapper = new Mock<IMapper>();

        _complaintImageService = new ComplaintImageService(
            _mockComplaintImageRepository.Object,
            _mockComplaintRepository.Object,
            _mockClaimsService.Object,
            _mockComplaintStatusRepository.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task AddComplaintImageAsync_WithValidComplaintAndOwner_ReturnsMappedComplaintImageDto()
    {
        // Arrange
        var userId = 5;
        var complaintId = 10;
        var complaintImageVm = CreateComplaintImageVm(complaintId, "http://image.jpg");
        var complaint = CreateComplaint(complaintId, userId);

        var addedComplaintImage = CreateComplaintImage(1, complaintId, "http://image.jpg");
        var expectedDto = CreateComplaintImageDto(1, complaintId, "http://image.jpg");

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockComplaintImageRepository
            .Setup(r => r.AddComplaintImageAsync(It.Is<ComplaintImage>(ci =>
                ci.ComplaintId == complaintImageVm.ComplaintId &&
                ci.ImageUrl == complaintImageVm.ImageUrl)))
            .ReturnsAsync(addedComplaintImage);

        _mockMapper
            .Setup(m => m.Map<ComplaintImageDto>(addedComplaintImage))
            .Returns(expectedDto);

        // Act
        var result = await _complaintImageService.AddComplaintImageAsync(complaintImageVm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.ComplaintImageId, result!.ComplaintImageId);
        Assert.Equal(expectedDto.ComplaintId, result.ComplaintId);
        Assert.Equal(expectedDto.ImageUrl, result.ImageUrl);

        _mockClaimsService.Verify(s => s.GetUserIdFromClaimsAsync(), Times.Once);
        _mockComplaintRepository.Verify(r => r.GetComplaintByComplaintIdAsync(complaintId), Times.Once);
        _mockComplaintImageRepository.Verify(r => r.AddComplaintImageAsync(It.IsAny<ComplaintImage>()), Times.Once);
        _mockMapper.Verify(m => m.Map<ComplaintImageDto>(addedComplaintImage), Times.Once);
    }

    [Fact]
    public async Task AddComplaintImageAsync_WithNonExistingComplaint_ReturnsNull()
    {
        // Arrange
        var userId = 5;
        var complaintId = 10;
        var complaintImageVm = CreateComplaintImageVm(complaintId, "http://image.jpg");

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync((Complaint?)null);

        // Act
        var result = await _complaintImageService.AddComplaintImageAsync(complaintImageVm);

        // Assert
        Assert.Null(result);
        _mockComplaintImageRepository.Verify(r => r.AddComplaintImageAsync(It.IsAny<ComplaintImage>()), Times.Never);
        _mockMapper.Verify(m => m.Map<ComplaintImageDto>(It.IsAny<ComplaintImage>()), Times.Never);
    }

    [Fact]
    public async Task AddComplaintImageAsync_WithNonOwnerUser_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var userId = 5;
        var complaintOwnerId = 99;
        var complaintId = 10;
        var complaintImageVm = CreateComplaintImageVm(complaintId, "http://image.jpg");

        var complaint = CreateComplaint(complaintId, complaintOwnerId);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintImageService.AddComplaintImageAsync(complaintImageVm));

        // Assert
        Assert.Equal("you cannot add", exception.Message);
        _mockComplaintImageRepository.Verify(r => r.AddComplaintImageAsync(It.IsAny<ComplaintImage>()), Times.Never);
        _mockMapper.Verify(m => m.Map<ComplaintImageDto>(It.IsAny<ComplaintImage>()), Times.Never);
    }

    [Fact]
    public async Task GetComplaintImageByIdAsync_WithExistingImage_ReturnsMappedDto()
    {
        // Arrange
        var complaintImageId = 1;
        var complaintImage = CreateComplaintImage(complaintImageId, 10, "http://image.jpg");
        var expectedDto = CreateComplaintImageDto(complaintImageId, 10, "http://image.jpg");

        _mockComplaintImageRepository
            .Setup(r => r.GetComplaintImageByIdAsync(complaintImageId))
            .ReturnsAsync(complaintImage);

        _mockMapper
            .Setup(m => m.Map<ComplaintImageDto>(complaintImage))
            .Returns(expectedDto);

        // Act
        var result = await _complaintImageService.GetComplaintImageByIdAsync(complaintImageId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.ComplaintImageId, result!.ComplaintImageId);
        Assert.Equal(expectedDto.ComplaintId, result.ComplaintId);
        Assert.Equal(expectedDto.ImageUrl, result.ImageUrl);

        _mockComplaintImageRepository.Verify(r => r.GetComplaintImageByIdAsync(complaintImageId), Times.Once);
        _mockMapper.Verify(m => m.Map<ComplaintImageDto>(complaintImage), Times.Once);
    }

    [Fact]
    public async Task GetComplaintImageByIdAsync_WithNonExistingImage_ReturnsNull()
    {
        // Arrange
        var complaintImageId = 1;

        _mockComplaintImageRepository
            .Setup(r => r.GetComplaintImageByIdAsync(complaintImageId))
            .ReturnsAsync((ComplaintImage?)null);

        // Act
        var result = await _complaintImageService.GetComplaintImageByIdAsync(complaintImageId);

        // Assert
        Assert.Null(result);
        _mockMapper.Verify(m => m.Map<ComplaintImageDto>(It.IsAny<ComplaintImage>()), Times.Never);
    }

    [Fact]
    public async Task GetComplaintImagesByComplaintIdAsync_WithExistingImages_ReturnsMappedDtoList()
    {
        // Arrange
        var complaintId = 10;

        var images = new List<ComplaintImage>
        {
            CreateComplaintImage(1, complaintId, "http://image1.jpg"),
            CreateComplaintImage(2, complaintId, "http://image2.jpg")
        };

        var dto1 = CreateComplaintImageDto(1, complaintId, "http://image1.jpg");
        var dto2 = CreateComplaintImageDto(2, complaintId, "http://image2.jpg");

        _mockComplaintImageRepository
            .Setup(r => r.GetAllComplaintImagesByComplaintIdAsync(complaintId))
            .ReturnsAsync(images);

        _mockMapper
            .Setup(m => m.Map<ComplaintImageDto>(images[0]))
            .Returns(dto1);

        _mockMapper
            .Setup(m => m.Map<ComplaintImageDto>(images[1]))
            .Returns(dto2);

        // Act
        var result = await _complaintImageService.GetComplaintImagesByComplaintIdAsync(complaintId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
        Assert.Equal(dto1.ComplaintImageId, result[0].ComplaintImageId);
        Assert.Equal(dto2.ComplaintImageId, result[1].ComplaintImageId);

        _mockComplaintImageRepository.Verify(r => r.GetAllComplaintImagesByComplaintIdAsync(complaintId), Times.Once);
        _mockMapper.Verify(m => m.Map<ComplaintImageDto>(It.IsAny<ComplaintImage>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetComplaintImagesByComplaintIdAsync_WithNullFromRepository_ReturnsNull()
    {
        // Arrange
        var complaintId = 10;

        _mockComplaintImageRepository
            .Setup(r => r.GetAllComplaintImagesByComplaintIdAsync(complaintId))
            .ReturnsAsync((List<ComplaintImage>)null!);

        // Act
        var result = await _complaintImageService.GetComplaintImagesByComplaintIdAsync(complaintId);

        // Assert
        Assert.Null(result);
        _mockMapper.Verify(m => m.Map<ComplaintImageDto>(It.IsAny<ComplaintImage>()), Times.Never);
    }

    [Fact]
    public async Task DeleteComplaintImageByIdAsync_WithValidUserAndUnderValidationStatus_ReturnsTrue()
    {
        // Arrange
        var complaintImageId = 1;
        var complaintId = 10;
        var userId = 5;

        var complaintImage = CreateComplaintImage(complaintImageId, complaintId, "http://image.jpg");
        var complaint = CreateComplaint(complaintId, userId);
        var complaintStatus = CreateComplaintStatus(complaintId, (int)Status.UnderValidation);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockComplaintImageRepository
            .Setup(r => r.GetComplaintImageByIdAsync(complaintImageId))
            .ReturnsAsync(complaintImage);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(
                It.Is<int?>(id => id == complaintId),
                It.IsAny<int?>()))
            .ReturnsAsync(complaintStatus);

        _mockComplaintImageRepository
            .Setup(r => r.RemoveComplaintImageAsync(complaintImage))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _complaintImageService.DeleteComplaintImageByIdAsync(complaintImageId);

        // Assert
        Assert.True(result);

        _mockComplaintImageRepository.Verify(r => r.GetComplaintImageByIdAsync(complaintImageId), Times.Once);
        _mockComplaintRepository.Verify(r => r.GetComplaintByComplaintIdAsync(complaintId), Times.Once);

        _mockComplaintStatusRepository.Verify(
            r => r.GetComplaintStatusAsync(
                It.Is<int?>(id => id == complaintId),
                It.IsAny<int?>()),
            Times.Once);

        _mockComplaintImageRepository.Verify(r => r.RemoveComplaintImageAsync(complaintImage), Times.Once);
    }

    [Fact]
    public async Task DeleteComplaintImageByIdAsync_WithNonExistingComplaintImage_ReturnsFalse()
    {
        // Arrange
        var complaintImageId = 1;
        var userId = 5;

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockComplaintImageRepository
            .Setup(r => r.GetComplaintImageByIdAsync(complaintImageId))
            .ReturnsAsync((ComplaintImage?)null);

        // Act
        var result = await _complaintImageService.DeleteComplaintImageByIdAsync(complaintImageId);

        // Assert
        Assert.False(result);

        _mockComplaintRepository.Verify(
            r => r.GetComplaintByComplaintIdAsync(It.IsAny<int>()),
            Times.Never);

        _mockComplaintStatusRepository.Verify(
            r => r.GetComplaintStatusAsync(It.IsAny<int?>(), It.IsAny<int?>()),
            Times.Never);

        _mockComplaintImageRepository.Verify(
            r => r.RemoveComplaintImageAsync(It.IsAny<ComplaintImage>()),
            Times.Never);
    }


    [Fact]
    public async Task DeleteComplaintImageByIdAsync_WhenUserIsNotOwner_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var complaintImageId = 1;
        var complaintId = 10;
        var loggedInUserId = 5;
        var complaintOwnerId = 99;

        var complaintImage = CreateComplaintImage(complaintImageId, complaintId, "http://image.jpg");
        var complaint = CreateComplaint(complaintId, complaintOwnerId);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(loggedInUserId);

        _mockComplaintImageRepository
            .Setup(r => r.GetComplaintImageByIdAsync(complaintImageId))
            .ReturnsAsync(complaintImage);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintImageService.DeleteComplaintImageByIdAsync(complaintImageId));

        // Assert
        Assert.Equal("you cannot delete", exception.Message);

        _mockComplaintStatusRepository.Verify(
            r => r.GetComplaintStatusAsync(It.IsAny<int?>(), It.IsAny<int?>()),
            Times.Never);

        _mockComplaintImageRepository.Verify(
            r => r.RemoveComplaintImageAsync(It.IsAny<ComplaintImage>()),
            Times.Never);
    }


    [Fact]
    public async Task DeleteComplaintImageByIdAsync_WhenStatusIsNotUnderValidation_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var complaintImageId = 1;
        var complaintId = 10;
        var userId = 5;

        var complaintImage = CreateComplaintImage(complaintImageId, complaintId, "http://image.jpg");
        var complaint = CreateComplaint(complaintId, userId);
        var complaintStatus = CreateComplaintStatus(complaintId, (int)Status.Backlog);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockComplaintImageRepository
            .Setup(r => r.GetComplaintImageByIdAsync(complaintImageId))
            .ReturnsAsync(complaintImage);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockComplaintStatusRepository
            .Setup(r => r.GetComplaintStatusAsync(
                It.Is<int?>(id => id == complaintId),
                It.IsAny<int?>()))
            .ReturnsAsync(complaintStatus);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _complaintImageService.DeleteComplaintImageByIdAsync(complaintImageId));

        // Assert
        Assert.Equal("This complaintImage cannot be delete at this stage", exception.Message);
        _mockComplaintImageRepository.Verify(
            r => r.RemoveComplaintImageAsync(It.IsAny<ComplaintImage>()),
            Times.Never);
    }


    private static ComplaintImageVm CreateComplaintImageVm(int complaintId, string imageUrl)
    {
        return new ComplaintImageVm
        {
            ComplaintId = complaintId,
            ImageUrl = imageUrl
        };
    }

    private static Complaint CreateComplaint(int complaintId, int citizenId)
    {
        return new Complaint
        {
            ComplaintId = complaintId,
            CitizenId = citizenId
        };
    }

    private static ComplaintImage CreateComplaintImage(int complaintImageId, int complaintId, string imageUrl)
    {
        return new ComplaintImage
        {
            ComplaintImageId = complaintImageId,
            ComplaintId = complaintId,
            ImageUrl = imageUrl,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };
    }

    private static ComplaintImageDto CreateComplaintImageDto(int complaintImageId, int complaintId, string imageUrl)
    {
        return new ComplaintImageDto
        {
            ComplaintImageId = complaintImageId,
            ComplaintId = complaintId,
            ImageUrl = imageUrl
        };
    }

    private static ComplaintStatus CreateComplaintStatus(int complaintId, int status)
    {
        return new ComplaintStatus
        {
            ComplaintId = complaintId,
            Status = status
        };
    }
}
