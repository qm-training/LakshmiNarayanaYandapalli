namespace WelfareWorkTracker.Tests.Infrastructure.Services;
public class ConstituencyServiceTests
{
    private readonly Mock<IConstituencyRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ConstituencyService _service;

    public ConstituencyServiceTests()
    {
        _mockRepo = new Mock<IConstituencyRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new ConstituencyService(_mockRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task AddConstituencyAsync_WithValidInput_ReturnsDto()
    {
        // Arrange
        var vm = new ConstituencyVm
        {
            ConstituencyName = "Vizag North",
            DistrictName = "Visakhapatnam",
            StateName = "Andhra Pradesh",
            CountryName = "India",
            Pincode = 530017
        };

        var added = CreateConstituency(1, vm);
        var mapped = CreateDto(1, vm);

        _mockRepo
            .Setup(r => r.AddConstituencyAsync(It.IsAny<Constituency>()))
            .ReturnsAsync(added);

        _mockMapper
            .Setup(m => m.Map<ConstituencyDto>(added))
            .Returns(mapped);

        // Act
        var result = await _service.AddConstituencyAsync(vm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mapped.ConstituencyId, result.ConstituencyId);
        Assert.Equal(mapped.ConstituencyName, result.ConstituencyName);
        Assert.Equal(mapped.DistrictName, result.DistrictName);
        Assert.Equal(mapped.StateName, result.StateName);
        Assert.Equal(mapped.CountryName, result.CountryName);
        Assert.Equal(mapped.Pincode, result.Pincode);

        _mockRepo.Verify(r => r.AddConstituencyAsync(It.Is<Constituency>(c =>
            c.ConstituencyName == vm.ConstituencyName &&
            c.DistrictName == vm.DistrictName &&
            c.StateName == vm.StateName &&
            c.CountryName == vm.CountryName &&
            c.Pincode == vm.Pincode
        )), Times.Once);

        _mockMapper.Verify(m => m.Map<ConstituencyDto>(added), Times.Once);
    }

    [Fact]
    public async Task GetAllConstituenciesAsync_WithData_ReturnsDtos()
    {
        // Arrange
        var vm1 = new ConstituencyVm
        {
            ConstituencyName = "Vijayawada Central",
            DistrictName = "Krishna",
            StateName = "Andhra Pradesh",
            CountryName = "India",
            Pincode = 520001
        };
        var vm2 = new ConstituencyVm
        {
            ConstituencyName = "Guntur West",
            DistrictName = "Guntur",
            StateName = "Andhra Pradesh",
            CountryName = "India",
            Pincode = 522002
        };

        var entities = new List<Constituency>
            {
                CreateConstituency(10, vm1),
                CreateConstituency(11, vm2)
            };

        var dto1 = CreateDto(10, vm1);
        var dto2 = CreateDto(11, vm2);

        _mockRepo.Setup(r => r.GetConstituenciesAsync()).ReturnsAsync(entities);

        _mockMapper.Setup(m => m.Map<ConstituencyDto>(entities[0])).Returns(dto1);
        _mockMapper.Setup(m => m.Map<ConstituencyDto>(entities[1])).Returns(dto2);

        // Act
        var result = await _service.GetAllConstituenciesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Contains(result, d => d.ConstituencyId == 10 && d.ConstituencyName == vm1.ConstituencyName);
        Assert.Contains(result, d => d.ConstituencyId == 11 && d.ConstituencyName == vm2.ConstituencyName);

        _mockRepo.Verify(r => r.GetConstituenciesAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<ConstituencyDto>(It.IsAny<Constituency>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetAllConstituenciesAsync_WithEmptyList_ThrowsNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetConstituenciesAsync()).ReturnsAsync(new List<Constituency>());

        // Act
        var act = () => _service.GetAllConstituenciesAsync();

        // Assert
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(act);
        Assert.Equal("No constituency found", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetAllConstituenciesAsync_WithNullList_ThrowsNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetConstituenciesAsync()).ReturnsAsync((List<Constituency>)null!);

        // Act
        var act = () => _service.GetAllConstituenciesAsync();

        // Assert
        var ex = await Assert.ThrowsAsync<WelfareWorkTrackerException>(act);
        Assert.Equal("No constituency found", ex.Message);
        Assert.Equal((int)HttpStatusCode.NotFound, ex.StatusCode);
    }
    private static Constituency CreateConstituency(int id, ConstituencyVm vm) =>
        new Constituency
        {
            ConstituencyId = id,
            ConstituencyName = vm.ConstituencyName,
            DistrictName = vm.DistrictName,
            StateName = vm.StateName,
            CountryName = vm.CountryName,
            Pincode = vm.Pincode
        };

    private static ConstituencyDto CreateDto(int id, ConstituencyVm vm) =>
        new ConstituencyDto
        {
            ConstituencyId = id,
            ConstituencyName = vm.ConstituencyName,
            DistrictName = vm.DistrictName,
            StateName = vm.StateName,
            CountryName = vm.CountryName,
            Pincode = vm.Pincode
        };
}
