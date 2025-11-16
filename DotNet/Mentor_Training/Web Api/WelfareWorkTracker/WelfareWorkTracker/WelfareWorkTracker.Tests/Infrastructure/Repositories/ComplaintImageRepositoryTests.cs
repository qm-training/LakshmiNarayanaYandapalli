namespace WelfareWorkTracker.Tests.Infrastructure.Repositories;
public class ComplaintImageRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly WelfareWorkTrackerContext _context;
    private readonly ComplaintImageRepository _repository;

    public ComplaintImageRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _repository = new ComplaintImageRepository(_context);
    }

    // AddComplaintImageAsync

    [Fact]
    public async Task AddComplaintImageAsync_WithValidImage_ReturnsPersistedImage()
    {
        // Arrange
        var image = BuildImage(complaintId: 10);

        // Act
        var result = await _repository.AddComplaintImageAsync(image);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ComplaintImageId > 0);
        Assert.Equal(10, result.ComplaintId);
        var fetched = await _context.ComplaintImages.FindAsync(result.ComplaintImageId);
        Assert.NotNull(fetched);
        Assert.Equal(result.ComplaintImageId, fetched!.ComplaintImageId);
    }

    [Fact]
    public async Task AddComplaintImageAsync_WithMinimalFields_ReturnsPersistedImage()
    {
        // Arrange
        var image = new ComplaintImage
        {
            ComplaintImageId = 0,
            ComplaintId = 5,
            ImageUrl = "image1"
        };

        // Act
        var result = await _repository.AddComplaintImageAsync(image);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ComplaintImageId > 0);
        Assert.Equal(5, result.ComplaintId);
    }

    // GetAllComplaintImagesByComplaintIdAsync

    [Fact]
    public async Task GetAllComplaintImagesByComplaintIdAsync_WithExistingImages_ReturnsOnlyThoseImages()
    {
        // Arrange
        var for1a = BuildImage(complaintId: 1);
        var for1b = BuildImage(complaintId: 1);
        var for2 = BuildImage(complaintId: 2);
        _context.ComplaintImages.AddRange(for1a, for1b, for2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllComplaintImagesByComplaintIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, i => Assert.Equal(1, i.ComplaintId));
    }

    [Fact]
    public async Task GetAllComplaintImagesByComplaintIdAsync_WithNoImages_ReturnsEmptyList()
    {
        // Arrange
        var forOther = BuildImage(complaintId: 9);
        _context.ComplaintImages.Add(forOther);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllComplaintImagesByComplaintIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // GetComplaintImageByIdAsync

    [Fact]
    public async Task GetComplaintImageByIdAsync_WithExistingId_ReturnsImage()
    {
        // Arrange
        var img = BuildImage(complaintId: 3);
        _context.ComplaintImages.Add(img);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintImageByIdAsync(img.ComplaintImageId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(img.ComplaintImageId, result!.ComplaintImageId);
        Assert.Equal(3, result.ComplaintId);
    }

    [Fact]
    public async Task GetComplaintImageByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var img = BuildImage(complaintId: 4);
        _context.ComplaintImages.Add(img);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComplaintImageByIdAsync(img.ComplaintImageId + 1000);

        // Assert
        Assert.Null(result);
    }

    // RemoveComplaintImageAsync

    [Fact]
    public async Task RemoveComplaintImageAsync_WithTrackedEntity_RemovesImage()
    {
        // Arrange
        var img = BuildImage(complaintId: 8);
        _context.ComplaintImages.Add(img);
        await _context.SaveChangesAsync();

        // Act
        await _repository.RemoveComplaintImageAsync(img);

        // Assert
        var deleted = await _context.ComplaintImages.FindAsync(img.ComplaintImageId);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task RemoveComplaintImageAsync_WithDetachedEntity_RemovesImage()
    {
        // Arrange
        var img = BuildImage(complaintId: 12);
        _context.ComplaintImages.Add(img);
        await _context.SaveChangesAsync();
        _context.Entry(img).State = EntityState.Detached;
        var toRemove = new ComplaintImage { ComplaintImageId = img.ComplaintImageId, ComplaintId = img.ComplaintId };

        // Act
        await _repository.RemoveComplaintImageAsync(toRemove);

        // Assert
        var deleted = await _context.ComplaintImages.FindAsync(img.ComplaintImageId);
        Assert.Null(deleted);
    }
    private ComplaintImage BuildImage(int complaintId)
    {
        return _fixture.Build<ComplaintImage>()
            .With(i => i.ComplaintImageId, 0)
            .With(i => i.ComplaintId, complaintId)
            .Create();
    }
}
