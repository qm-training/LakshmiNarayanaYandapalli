namespace WelfareWorkTracker.Tests.Controllers;
public class CommentControllerTests
{
    private readonly CommentController _controller;
    private readonly Mock<ICommentService> _mockService;

    public CommentControllerTests()
    {
        _mockService = new Mock<ICommentService>();
        _controller = new CommentController(_mockService.Object);
    }

    [Fact]
    public async Task AddCommentById_WithValidComplaintId_ReturnsOk()
    {
        // Arrange
        var vm = CreateCommentVm();
        var created = new CommentDto { CommentId = 10, Description = vm.Description, ComplaintId = 1 };
        _mockService
            .Setup(s => s.AddCommentAsync(1, null, vm))
            .ReturnsAsync(created);

        // Act
        var result = await _controller.AddCommentById(1, null, vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal("comment added sucessfully", ok.Value);
    }

    [Fact]
    public async Task AddCommentById_WithNonExistingComplaintId_ReturnsNotFound()
    {
        // Arrange
        var vm = CreateCommentVm();
        _mockService
            .Setup(s => s.AddCommentAsync(999, null, vm))
            .ReturnsAsync((CommentDto?)null);

        // Act
        var result = await _controller.AddCommentById(999, null, vm);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Complaint not found with complaintId: 999", notFound.Value);
    }

    [Fact]
    public async Task AddCommentById_WithNonExistingDailyComplaintId_ReturnsNotFound()
    {
        // Arrange
        var vm = CreateCommentVm();
        _mockService
            .Setup(s => s.AddCommentAsync(null, 777, vm))
            .ReturnsAsync((CommentDto?)null);

        // Act
        var result = await _controller.AddCommentById(null, 777, vm);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Daily Complaint not found with daily complaintId: 777", notFound.Value);
    }

    [Fact]
    public async Task GetCommentsById_WithExistingIds_ReturnsOkWithComments()
    {
        // Arrange
        var comments = new List<CommentDto>
        {
            new CommentDto { CommentId = 1, Description = "A", ComplaintId = 1 },
            new CommentDto { CommentId = 2, Description = "B", ComplaintId = 1 }
        };
        _mockService
            .Setup(s => s.GetCommentsByIdAsync(1, null))
            .ReturnsAsync(comments);

        // Act
        var result = await _controller.GetCommentsById(1, null);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal(comments, ok.Value);
    }

    [Fact]
    public async Task GetCommentsById_WhenNoComments_ReturnsOkWithNull()
    {
        // Arrange
        _mockService
            .Setup(s => s.GetCommentsByIdAsync(null, 55))
            .ReturnsAsync(new List<CommentDto>());

        // Act
        var result = await _controller.GetCommentsById(null, 55);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        var value = Assert.IsAssignableFrom<IEnumerable<CommentDto>>(ok.Value);
        Assert.Empty(value);
    }

    [Fact]
    public async Task UpdateCommentByCommentId_WithExistingComment_ReturnsNoContent()
    {
        // Arrange
        var vm = CreateCommentVm("Updated");
        var updated = new CommentDto { CommentId = 9, Description = "Updated", ComplaintId = 1 };
        _mockService
            .Setup(s => s.UpdateCommentByIdAsync(9, vm))
            .ReturnsAsync(updated);

        // Act
        var result = await _controller.UpdateCommentByCommentId(9, vm);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateCommentByCommentId_WhenCommentNotFound_ReturnsNotFound()
    {
        // Arrange
        var vm = CreateCommentVm("Nope");
        _mockService
            .Setup(s => s.UpdateCommentByIdAsync(404, vm))
            .ReturnsAsync((CommentDto)null!);

        // Act
        var result = await _controller.UpdateCommentByCommentId(404, vm);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Comment Not Found.", notFound.Value);
    }

    [Fact]
    public async Task DeleteCommentByCommentId_WithExistingComment_ReturnsNoContent()
    {
        // Arrange
        _mockService
            .Setup(s => s.DeleteCommentByCommentIdAsync(5))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteCommentByCommentId(5);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteCommentByCommentId_WhenCommentNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService
            .Setup(s => s.DeleteCommentByCommentIdAsync(123))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteCommentByCommentId(123);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var value = notFound.Value;
        var messageProperty = value!.GetType().GetProperty("message")?.GetValue(value, null);
        Assert.Equal("Comment not found.", messageProperty);
    }

    private static CommentVm CreateCommentVm(string description = "Test comment")
        => new CommentVm { Description = description, IsAnonymous = false};
}
