namespace WelfareWorkTracker.Tests.Infrastructure.Services;
public class CommentServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICommentRepository> _mockCommentRepository;
    private readonly Mock<IComplaintRepository> _mockComplaintRepository;
    private readonly Mock<IDailyComplaintRepository> _mockDailyComplaintRepository;
    private readonly Mock<IClaimsService> _mockClaimsService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CommentService _commentService;

    public CommentServiceTests()
    {
        _fixture = new Fixture();
        _mockCommentRepository = new Mock<ICommentRepository>();
        _mockComplaintRepository = new Mock<IComplaintRepository>();
        _mockDailyComplaintRepository = new Mock<IDailyComplaintRepository>();
        _mockClaimsService = new Mock<IClaimsService>();
        _mockMapper = new Mock<IMapper>();

        _commentService = new CommentService(
            _mockCommentRepository.Object,
            _mockComplaintRepository.Object,
            _mockDailyComplaintRepository.Object,
            _mockClaimsService.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task AddCommentAsync_WithoutAnyIds_ThrowsBadRequestException()
    {
        // Arrange
        var commentVm = CreateCommentVm();
        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(1);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _commentService.AddCommentAsync(null, null, commentVm));

        // Assert
        Assert.Equal("Any one of the ID's should be provided.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
        _mockCommentRepository.Verify(r => r.AddCommentByIdAsync(It.IsAny<Comment>()), Times.Never);
    }

    [Fact]
    public async Task AddCommentAsync_WithBothComplaintIdAndDailyComplaintId_ThrowsBadRequestException()
    {
        // Arrange
        var commentVm = CreateCommentVm();
        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(1);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _commentService.AddCommentAsync(1, 2, commentVm));

        // Assert
        Assert.Equal("only one of the ID's should be provided.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
        _mockCommentRepository.Verify(r => r.AddCommentByIdAsync(It.IsAny<Comment>()), Times.Never);
    }

    [Fact]
    public async Task AddCommentAsync_WithComplaintIdAndExistingComplaint_ReturnsMappedCommentDto()
    {
        // Arrange
        var complaintId = 10;
        int? dailyComplaintId = null;
        var commentVm = CreateCommentVm();
        var userId = 5;

        var complaint = CreateComplaint(complaintId);
        var addedComment = CreateCommentEntity(1, userId, complaintId, null, commentVm.Description, commentVm.IsAnonymous);
        var expectedDto = CreateCommentDto(1, commentVm.Description, commentVm.IsAnonymous);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync(complaint);

        _mockCommentRepository
            .Setup(r => r.AddCommentByIdAsync(It.Is<Comment>(c =>
                c.UserId == userId &&
                c.ComplaintId == complaintId &&
                c.DailyComplaintId == dailyComplaintId &&
                c.Description == commentVm.Description &&
                c.IsAnonymous == commentVm.IsAnonymous)))
            .ReturnsAsync(addedComment);

        _mockMapper
            .Setup(m => m.Map<CommentDto>(addedComment))
            .Returns(expectedDto);

        // Act
        var result = await _commentService.AddCommentAsync(complaintId, dailyComplaintId, commentVm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.CommentId, result!.CommentId);
        Assert.Equal(expectedDto.Description, result.Description);
        Assert.Equal(expectedDto.IsAnonymous, result.IsAnonymous);

        _mockComplaintRepository.Verify(r => r.GetComplaintByComplaintIdAsync(complaintId), Times.Once);
        _mockDailyComplaintRepository.Verify(r => r.GetDailyComplaintByIdAsync(It.IsAny<int>()), Times.Never);
        _mockCommentRepository.Verify(r => r.AddCommentByIdAsync(It.IsAny<Comment>()), Times.Once);
        _mockMapper.Verify(m => m.Map<CommentDto>(addedComment), Times.Once);
    }

    [Fact]
    public async Task AddCommentAsync_WithComplaintIdAndNonExistingComplaint_ReturnsNull()
    {
        // Arrange
        var complaintId = 10;
        int? dailyComplaintId = null;
        var commentVm = CreateCommentVm();

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(5);

        _mockComplaintRepository
            .Setup(r => r.GetComplaintByComplaintIdAsync(complaintId))
            .ReturnsAsync((Complaint?)null);

        // Act
        var result = await _commentService.AddCommentAsync(complaintId, dailyComplaintId, commentVm);

        // Assert
        Assert.Null(result);
        _mockCommentRepository.Verify(r => r.AddCommentByIdAsync(It.IsAny<Comment>()), Times.Never);
    }

    [Fact]
    public async Task AddCommentAsync_WithDailyComplaintIdAndExistingDailyComplaint_ReturnsMappedCommentDto()
    {
        // Arrange
        int? complaintId = null;
        var dailyComplaintId = 20;
        var commentVm = CreateCommentVm();
        var userId = 7;

        var dailyComplaint = CreateDailyComplaint(dailyComplaintId);
        var addedComment = CreateCommentEntity(1, userId, null, dailyComplaintId, commentVm.Description, commentVm.IsAnonymous);
        var expectedDto = CreateCommentDto(1, commentVm.Description, commentVm.IsAnonymous);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockDailyComplaintRepository
            .Setup(r => r.GetDailyComplaintByIdAsync(dailyComplaintId))
            .ReturnsAsync(dailyComplaint);

        _mockCommentRepository
            .Setup(r => r.AddCommentByIdAsync(It.Is<Comment>(c =>
                c.UserId == userId &&
                c.ComplaintId == complaintId &&
                c.DailyComplaintId == dailyComplaintId &&
                c.Description == commentVm.Description &&
                c.IsAnonymous == commentVm.IsAnonymous)))
            .ReturnsAsync(addedComment);

        _mockMapper
            .Setup(m => m.Map<CommentDto>(addedComment))
            .Returns(expectedDto);

        // Act
        var result = await _commentService.AddCommentAsync(complaintId, dailyComplaintId, commentVm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.CommentId, result!.CommentId);
        Assert.Equal(expectedDto.Description, result.Description);
        Assert.Equal(expectedDto.IsAnonymous, result.IsAnonymous);

        _mockDailyComplaintRepository.Verify(r => r.GetDailyComplaintByIdAsync(dailyComplaintId), Times.Once);
        _mockComplaintRepository.Verify(r => r.GetComplaintByComplaintIdAsync(It.IsAny<int>()), Times.Never);
        _mockCommentRepository.Verify(r => r.AddCommentByIdAsync(It.IsAny<Comment>()), Times.Once);
        _mockMapper.Verify(m => m.Map<CommentDto>(addedComment), Times.Once);
    }

    [Fact]
    public async Task AddCommentAsync_WithDailyComplaintIdAndNonExistingDailyComplaint_ReturnsNull()
    {
        // Arrange
        int? complaintId = null;
        var dailyComplaintId = 20;
        var commentVm = CreateCommentVm();

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(7);

        _mockDailyComplaintRepository
            .Setup(r => r.GetDailyComplaintByIdAsync(dailyComplaintId))
            .ReturnsAsync((DailyComplaint?)null);

        // Act
        var result = await _commentService.AddCommentAsync(complaintId, dailyComplaintId, commentVm);

        // Assert
        Assert.Null(result);
        _mockCommentRepository.Verify(r => r.AddCommentByIdAsync(It.IsAny<Comment>()), Times.Never);
    }

    [Fact]
    public async Task GetCommentsByIdAsync_WithComplaintId_ReturnsMappedComments()
    {
        // Arrange
        var complaintId = 10;
        int? dailyComplaintId = null;
        var userId = 5;

        var comments = _fixture.CreateMany<Comment>(3).ToList();
        var expectedDtos = _fixture.CreateMany<CommentDto>(3).ToList();

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockCommentRepository
            .Setup(r => r.GetCommentsByIdAsync(complaintId, dailyComplaintId, userId))
            .ReturnsAsync(comments);

        _mockMapper
            .Setup(m => m.Map<List<CommentDto>>(comments))
            .Returns(expectedDtos);

        // Act
        var result = await _commentService.GetCommentsByIdAsync(complaintId, dailyComplaintId);

        // Assert
        Assert.Equal(expectedDtos.Count, result.Count);
        _mockCommentRepository.Verify(r => r.GetCommentsByIdAsync(complaintId, dailyComplaintId, userId), Times.Once);
        _mockMapper.Verify(m => m.Map<List<CommentDto>>(comments), Times.Once);
    }

    [Fact]
    public async Task GetCommentsByIdAsync_WithoutAnyIds_ThrowsBadRequestException()
    {
        // Arrange
        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(1);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _commentService.GetCommentsByIdAsync(null, null));

        // Assert
        Assert.Equal("Any one of the ID's should be provided.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
        _mockCommentRepository.Verify(r => r.GetCommentsByIdAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetCommentsByIdAsync_WithBothIds_ThrowsBadRequestException()
    {
        // Arrange
        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(1);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _commentService.GetCommentsByIdAsync(1, 2));

        // Assert
        Assert.Equal("only one of the ID's should be provided.", exception.Message);
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
        _mockCommentRepository.Verify(r => r.GetCommentsByIdAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCommentByIdAsync_WithValidUser_UpdatesAndReturnsMappedComment()
    {
        // Arrange
        var commentId = 10;
        var userId = 5;
        var commentVm = CreateCommentVm();

        var existingComment = CreateCommentEntity(commentId, userId, 1, null, "old desc", false);
        var updatedComment = CreateCommentEntity(commentId, userId, 1, null, commentVm.Description, commentVm.IsAnonymous);
        var expectedDto = CreateCommentDto(commentId, commentVm.Description, commentVm.IsAnonymous);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockCommentRepository
            .Setup(r => r.GetCommentByIdAsync(commentId))
            .ReturnsAsync(existingComment);

        _mockCommentRepository
            .Setup(r => r.UpdateCommentByIdAsync(existingComment))
            .ReturnsAsync(updatedComment);

        _mockMapper
            .Setup(m => m.Map<CommentDto>(updatedComment))
            .Returns(expectedDto);

        // Act
        var result = await _commentService.UpdateCommentByIdAsync(commentId, commentVm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.CommentId, result.CommentId);
        Assert.Equal(expectedDto.Description, result.Description);
        Assert.Equal(expectedDto.IsAnonymous, result.IsAnonymous);

        _mockCommentRepository.Verify(r => r.GetCommentByIdAsync(commentId), Times.Once);
        _mockCommentRepository.Verify(r => r.UpdateCommentByIdAsync(existingComment), Times.Once);
        _mockMapper.Verify(m => m.Map<CommentDto>(updatedComment), Times.Once);
    }

    [Fact]
    public async Task UpdateCommentByIdAsync_WhenCommentNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var commentId = 10;
        var commentVm = CreateCommentVm();

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(5);

        _mockCommentRepository
            .Setup(r => r.GetCommentByIdAsync(commentId))
            .ReturnsAsync((Comment?)null);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _commentService.UpdateCommentByIdAsync(commentId, commentVm));

        // Assert
        Assert.Equal("comment not found", exception.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
        _mockCommentRepository.Verify(r => r.UpdateCommentByIdAsync(It.IsAny<Comment>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCommentByIdAsync_WhenCommentBelongsToDifferentUser_ThrowsUnauthorizedException()
    {
        // Arrange
        var commentId = 10;
        var commentVm = CreateCommentVm();
        var loggedInUserId = 5;
        var commentOwnerId = 99;

        var existingComment = CreateCommentEntity(commentId, commentOwnerId, 1, null, "old", false);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(loggedInUserId);

        _mockCommentRepository
            .Setup(r => r.GetCommentByIdAsync(commentId))
            .ReturnsAsync(existingComment);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _commentService.UpdateCommentByIdAsync(commentId, commentVm));

        // Assert
        Assert.Equal("you cannot update", exception.Message);
        Assert.Equal((int)HttpStatusCode.Unauthorized, exception.StatusCode);
        _mockCommentRepository.Verify(r => r.UpdateCommentByIdAsync(It.IsAny<Comment>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCommentByCommentIdAsync_WithValidUserAndDeletedComment_ReturnsTrue()
    {
        // Arrange
        var commentId = 10;
        var userId = 5;

        var existingComment = CreateCommentEntity(commentId, userId, 1, null, "desc", false);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockCommentRepository
            .Setup(r => r.GetCommentByIdAsync(commentId))
            .ReturnsAsync(existingComment);

        _mockCommentRepository
            .Setup(r => r.DeleteCommentAsync(existingComment))
            .ReturnsAsync(existingComment);

        // Act
        var result = await _commentService.DeleteCommentByCommentIdAsync(commentId);

        // Assert
        Assert.True(result);
        _mockCommentRepository.Verify(r => r.GetCommentByIdAsync(commentId), Times.Once);
        _mockCommentRepository.Verify(r => r.DeleteCommentAsync(existingComment), Times.Once);
    }

    [Fact]
    public async Task DeleteCommentByCommentIdAsync_WhenCommentNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var commentId = 10;

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(5);

        _mockCommentRepository
            .Setup(r => r.GetCommentByIdAsync(commentId))
            .ReturnsAsync((Comment?)null);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _commentService.DeleteCommentByCommentIdAsync(commentId));

        // Assert
        Assert.Equal("comment not found", exception.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
        _mockCommentRepository.Verify(r => r.DeleteCommentAsync(It.IsAny<Comment>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCommentByCommentIdAsync_WhenCommentBelongsToDifferentUser_ThrowsUnauthorizedException()
    {
        // Arrange
        var commentId = 10;
        var loggedInUserId = 5;
        var commentOwnerId = 20;

        var existingComment = CreateCommentEntity(commentId, commentOwnerId, 1, null, "desc", false);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(loggedInUserId);

        _mockCommentRepository
            .Setup(r => r.GetCommentByIdAsync(commentId))
            .ReturnsAsync(existingComment);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() =>
            _commentService.DeleteCommentByCommentIdAsync(commentId));

        // Assert
        Assert.Equal("you cannot delete", exception.Message);
        Assert.Equal((int)HttpStatusCode.Unauthorized, exception.StatusCode);
        _mockCommentRepository.Verify(r => r.DeleteCommentAsync(It.IsAny<Comment>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCommentByCommentIdAsync_WhenDeleteReturnsNull_ReturnsFalse()
    {
        // Arrange
        var commentId = 10;
        var userId = 5;

        var existingComment = CreateCommentEntity(commentId, userId, 1, null, "desc", false);

        _mockClaimsService
            .Setup(s => s.GetUserIdFromClaimsAsync())
            .ReturnsAsync(userId);

        _mockCommentRepository
            .Setup(r => r.GetCommentByIdAsync(commentId))
            .ReturnsAsync(existingComment);

        _mockCommentRepository
            .Setup(r => r.DeleteCommentAsync(existingComment))
            .ReturnsAsync((Comment?)null);

        // Act
        var result = await _commentService.DeleteCommentByCommentIdAsync(commentId);

        // Assert
        Assert.False(result);
        _mockCommentRepository.Verify(r => r.DeleteCommentAsync(existingComment), Times.Once);
    }

    private static CommentVm CreateCommentVm()
    {
        return new CommentVm
        {
            Description = "Test comment",
            IsAnonymous = false
        };
    }

    private static Complaint CreateComplaint(int complaintId)
    {
        return new Complaint
        {
            ComplaintId = complaintId
        };
    }

    private static DailyComplaint CreateDailyComplaint(int dailyComplaintId)
    {
        return new DailyComplaint
        {
            DailyComplaintId = dailyComplaintId
        };
    }

    private static Comment CreateCommentEntity(int commentId, int userId, int? complaintId, int? dailyComplaintId, string description, bool isAnonymous)
    {
        return new Comment
        {
            CommentId = commentId,
            UserId = userId,
            ComplaintId = complaintId,
            DailyComplaintId = dailyComplaintId,
            Description = description,
            IsAnonymous = isAnonymous,
            DateCreated = DateTime.UtcNow
        };
    }

    private static CommentDto CreateCommentDto(int commentId, string description, bool isAnonymous)
    {
        return new CommentDto
        {
            CommentId = commentId,
            Description = description,
            IsAnonymous = isAnonymous
        };
    }
}
