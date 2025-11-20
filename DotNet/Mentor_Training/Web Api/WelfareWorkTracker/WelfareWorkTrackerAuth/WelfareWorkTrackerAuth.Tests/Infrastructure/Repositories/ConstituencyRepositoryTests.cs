namespace WelfareWorkTrackerAuth.Tests.Infrastructure.Repositories;
public class ConstituencyRepositoryTests
{
    private readonly WelfareWorkTrackerContext _context;
    private readonly ConstituencyRepository _constituencyRepository;

    public ConstituencyRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WelfareWorkTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new WelfareWorkTrackerContext(options);
        _constituencyRepository = new ConstituencyRepository(_context);
    }

    [Fact]
    public async Task GetConstituencyIdByNameAsync_WithExistingName_ReturnsConstituencyId()
    {
        // Arrange
        var constituencyName = "Vijayawada Central";
        var createdConstituency = await AddConstituencyAsync(constituencyName);

        // Act
        var result = await _constituencyRepository.GetConstituencyIdByNameAsync(constituencyName);

        // Assert
        Assert.Equal(createdConstituency.ConstituencyId, result);
    }

    [Fact]
    public async Task GetConstituencyIdByNameAsync_WithNonExistingName_ReturnsZero()
    {
        // Arrange
        await AddConstituencyAsync("Some Other Constituency");
        var nonExistingName = "Non Existing Constituency";

        // Act
        var result = await _constituencyRepository.GetConstituencyIdByNameAsync(nonExistingName);

        // Assert
        Assert.Equal(0, result);
    }

    private async Task<Constituency> AddConstituencyAsync(string name)
    {
        var constituency = new Constituency
        {
            ConstituencyName = name,
            DistrictName = "Krishna",
            StateName = "Andhra Pradesh",
            CountryName = "India",
            Pincode = 520001
        };

        _context.Constituencies.Add(constituency);
        await _context.SaveChangesAsync();

        return constituency;
    }
}
