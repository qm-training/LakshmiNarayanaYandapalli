namespace WelfareWorkTracker.Tests.Infrastructure.Repositories;
public class ConstituencyRepositoryTests
{
    private readonly Fixture _fixture = new();
    private readonly WelfareWorkTrackerContext _context;
    private readonly ConstituencyRepository _repository;

    public ConstituencyRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _repository = new ConstituencyRepository(_context);
    }

    // AddConstituencyAsync

    [Fact]
    public async Task AddConstituencyAsync_WithValidConstituency_ReturnsPersistedConstituency()
    {
        // Arrange
        var entity = BuildConstituency("Central");

        // Act
        var result = await _repository.AddConstituencyAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ConstituencyId > 0);
        var fetched = await _context.Constituencies.FindAsync(result.ConstituencyId);
        Assert.NotNull(fetched);
        Assert.Equal("Central", fetched!.ConstituencyName);
    }

    [Fact]
    public async Task AddConstituencyAsync_WithMinimalValidFields_PersistsAndSetsKey()
    {
        // Arrange
        var entity = new Constituency
        {
            ConstituencyId = 0,
            ConstituencyName = "Harbor",
            DistrictName = "Krishna",
            StateName = "Andhra Pradesh",
            CountryName = "India"
        };

        // Act
        var result = await _repository.AddConstituencyAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ConstituencyId > 0);
        Assert.Equal("Harbor", result.ConstituencyName);
    }

    // GetConstituenciesAsync

    [Fact]
    public async Task GetConstituenciesAsync_WithExistingData_ReturnsAll()
    {
        // Arrange
        var a = BuildConstituency("Alpha");
        var b = BuildConstituency("Beta");
        _context.Constituencies.AddRange(a, b);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetConstituenciesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count >= 2);
        Assert.Contains(result, c => c.ConstituencyName == "Alpha");
        Assert.Contains(result, c => c.ConstituencyName == "Beta");
    }

    [Fact]
    public async Task GetConstituenciesAsync_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange

        // Act
        var result = await _repository.GetConstituenciesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // GetConstituencyIdByNameAsync

    [Fact]
    public async Task GetConstituencyIdByNameAsync_WithExistingName_ReturnsId()
    {
        // Arrange
        var entity = BuildConstituency("GreenPark");
        _context.Constituencies.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetConstituencyIdByNameAsync("GreenPark");

        // Assert
        Assert.Equal(entity.ConstituencyId, result);
    }

    [Fact]
    public async Task GetConstituencyIdByNameAsync_WithNonExistingName_ReturnsZero()
    {
        // Arrange
        var entity = BuildConstituency("RiverSide");
        _context.Constituencies.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetConstituencyIdByNameAsync("NoMatch");

        // Assert
        Assert.Equal(0, result);
    }

    // GetConstituencyNameByIdAsync

    [Fact]
    public async Task GetConstituencyNameByIdAsync_WithExistingId_ReturnsName()
    {
        // Arrange
        var entity = BuildConstituency("OldTown");
        _context.Constituencies.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetConstituencyNameByIdAsync(entity.ConstituencyId);

        // Assert
        Assert.Equal("OldTown", result);
    }

    [Fact]
    public async Task GetConstituencyNameByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var entity = BuildConstituency("HillSide");
        _context.Constituencies.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetConstituencyNameByIdAsync(entity.ConstituencyId + 1234);

        // Assert
        Assert.Null(result);
    }

    private Constituency BuildConstituency(string name)
    {
        return _fixture.Build<Constituency>()
            .With(c => c.ConstituencyId, 0)
            .With(c => c.ConstituencyName, name)
            .Create();
    }
}
