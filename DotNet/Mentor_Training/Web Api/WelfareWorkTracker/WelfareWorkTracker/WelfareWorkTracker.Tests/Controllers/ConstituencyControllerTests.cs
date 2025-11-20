namespace WelfareWorkTracker.Tests.Controllers;
public class ConstituencyControllerTests
{
    private readonly Mock<IConstituencyService> _mockService;
    private readonly ConstituencyController _controller;

    public ConstituencyControllerTests()
    {
        _mockService = new Mock<IConstituencyService>();
        _controller = new ConstituencyController(_mockService.Object);
    }

    [Fact]
    public async Task GetConstituencies_WithExistingData_ReturnsOkWithList()
    {
        // Arrange
        var list = new List<ConstituencyDto>
        {
            new ConstituencyDto { ConstituencyId = 1, ConstituencyName = "Guntur West", DistrictName = "Guntur", StateName = "Andhra Pradesh", CountryName = "India", Pincode = 522002 },
            new ConstituencyDto { ConstituencyId = 2, ConstituencyName = "Vizag North", DistrictName = "Visakhapatnam", StateName = "Andhra Pradesh", CountryName = "India", Pincode = 530017 }
        };
        _mockService.Setup(s => s.GetAllConstituenciesAsync()).ReturnsAsync(list);

        // Act
        var result = await _controller.GetConstituencies();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal(list, ok.Value);
    }

    [Fact]
    public async Task GetConstituencies_WhenEmpty_ReturnsOkWithEmptyList()
    {
        // Arrange
        var empty = new List<ConstituencyDto>();
        _mockService.Setup(s => s.GetAllConstituenciesAsync()).ReturnsAsync(empty);

        // Act
        var result = await _controller.GetConstituencies();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal(empty, ok.Value);
    }

    [Fact]
    public async Task AddConstituency_WithValidInput_ReturnsOkWithDto()
    {
        // Arrange
        var vm = CreateConstituencyVm();
        var dto = new ConstituencyDto
        {
            ConstituencyId = 10,
            ConstituencyName = vm.ConstituencyName,
            DistrictName = vm.DistrictName,
            StateName = vm.StateName,
            CountryName = vm.CountryName,
            Pincode = vm.Pincode
        };
        _mockService.Setup(s => s.AddConstituencyAsync(vm)).ReturnsAsync(dto);

        // Act
        var result = await _controller.AddConstituency(vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task AddConstituency_WhenServiceReturnsNull_ReturnsOkWithNull()
    {
        // Arrange
        var vm = CreateConstituencyVm();
        _mockService.Setup(s => s.AddConstituencyAsync(vm)).ReturnsAsync((ConstituencyDto)null!);

        // Act
        var result = await _controller.AddConstituency(vm);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Null(ok.Value);
    }

    private static ConstituencyVm CreateConstituencyVm()
        => new ConstituencyVm
        {
            ConstituencyName = "Vijayawada Central",
            DistrictName = "Krishna",
            StateName = "Andhra Pradesh",
            CountryName = "India",
            Pincode = 520001
        };
}
