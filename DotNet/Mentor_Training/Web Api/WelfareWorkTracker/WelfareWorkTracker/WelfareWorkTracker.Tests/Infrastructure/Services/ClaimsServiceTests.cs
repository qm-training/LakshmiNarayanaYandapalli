namespace WelfareWorkTracker.Tests.Infrastructure.Services;
public class ClaimsServiceTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IConstituencyRepository> _mockConstituencyRepository;
    private readonly ClaimsService _claimsService;
    private readonly DefaultHttpContext _httpContext;

    public ClaimsServiceTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockConstituencyRepository = new Mock<IConstituencyRepository>();

        _httpContext = new DefaultHttpContext();
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(_httpContext);

        _claimsService = new ClaimsService(
            _mockHttpContextAccessor.Object,
            _mockUserRepository.Object,
            _mockConstituencyRepository.Object);
    }

    [Fact]
    public void Constructor_WithNullHttpContextAccessor_ThrowsArgumentNullException()
    {
        // Arrange
        IHttpContextAccessor httpContextAccessor = null!;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new ClaimsService(httpContextAccessor, _mockUserRepository.Object, _mockConstituencyRepository.Object));

        // Assert
        Assert.Equal("httpContext", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullUserRepository_ThrowsArgumentNullException()
    {
        // Arrange
        IUserRepository userRepository = null!;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new ClaimsService(_mockHttpContextAccessor.Object, userRepository, _mockConstituencyRepository.Object));

        // Assert
        Assert.Equal("userRepository", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullConstituencyRepository_ThrowsArgumentNullException()
    {
        // Arrange
        IConstituencyRepository constituencyRepository = null!;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new ClaimsService(_mockHttpContextAccessor.Object, _mockUserRepository.Object, constituencyRepository));

        // Assert
        Assert.Equal("constituencyRepository", exception.ParamName);
    }

    [Fact]
    public void GetRoleNameFromClaims_WithValidRoleClaim_ReturnsRoleName()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new("Role", "Admin")
        };
        SetClaimsPrincipalWithClaims(claims);

        // Act
        var result = _claimsService.GetRoleNameFromClaims();

        // Assert
        Assert.Equal("Admin", result);
    }

    [Fact]
    public void GetRoleNameFromClaims_WithMissingRoleClaim_ThrowsInvalidOperationException()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new("Email", "user@example.com")
        };
        SetClaimsPrincipalWithClaims(claims);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => _claimsService.GetRoleNameFromClaims());

        // Assert
        Assert.Equal("Role claim not found.", exception.Message);
    }

    [Fact]
    public void GetRoleNameFromClaims_WhenHttpContextUserIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => _claimsService.GetRoleNameFromClaims());

        // Assert
        Assert.Equal("HttpContext or User is null", exception.Message);
    }

    [Fact]
    public async Task GetUserConstituencyFromClaimsAsync_WithValidEmailAndUserAndConstituency_ReturnsConstituencyName()
    {
        // Arrange
        var email = "user@example.com";
        var claims = new List<Claim>
        {
            new("Email", email)
        };
        SetClaimsPrincipalWithClaims(claims);

        var user = CreateUser(userId: 1, constituencyId: 10, email: email);
        const string constituencyName = "Test Constituency";

        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(email))
            .ReturnsAsync(user);

        _mockConstituencyRepository
            .Setup(r => r.GetConstituencyNameByIdAsync(user.ConstituencyId))
            .ReturnsAsync(constituencyName);

        // Act
        var result = await _claimsService.GetUserConstituencyFromClaimsAsync();

        // Assert
        Assert.Equal(constituencyName, result);
        _mockUserRepository.Verify(r => r.GetUserByEmailAsync(email), Times.Once);
        _mockConstituencyRepository.Verify(r => r.GetConstituencyNameByIdAsync(user.ConstituencyId), Times.Once);
    }

    [Fact]
    public async Task GetUserConstituencyFromClaimsAsync_WithMissingEmailClaim_ThrowsInvalidOperationException()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new("Role", "Admin")
        };
        SetClaimsPrincipalWithClaims(claims);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _claimsService.GetUserConstituencyFromClaimsAsync());

        // Assert
        Assert.Equal("Email claim not found.", exception.Message);
        _mockUserRepository.Verify(r => r.GetUserByEmailAsync(It.IsAny<string>()), Times.Never);
        _mockConstituencyRepository.Verify(r => r.GetConstituencyNameByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetUserConstituencyFromClaimsAsync_WithUserNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var email = "missing@example.com";
        var claims = new List<Claim>
        {
            new("Email", email)
        };
        SetClaimsPrincipalWithClaims(claims);

        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(email))
            .ReturnsAsync((User?)null);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _claimsService.GetUserConstituencyFromClaimsAsync());

        // Assert
        Assert.Equal("User not found.", exception.Message);
        _mockConstituencyRepository.Verify(r => r.GetConstituencyNameByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetUserConstituencyFromClaimsAsync_WithConstituencyNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var email = "user@example.com";
        var claims = new List<Claim>
        {
            new("Email", email)
        };
        SetClaimsPrincipalWithClaims(claims);

        var user = CreateUser(userId: 1, constituencyId: 10, email: email);

        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(email))
            .ReturnsAsync(user);

        _mockConstituencyRepository
            .Setup(r => r.GetConstituencyNameByIdAsync(user.ConstituencyId))
            .ReturnsAsync((string?)null);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _claimsService.GetUserConstituencyFromClaimsAsync());

        // Assert
        Assert.Equal("Constituency not found.", exception.Message);
    }

    [Fact]
    public async Task GetUserIdFromClaimsAsync_WithValidEmailAndUser_ReturnsUserId()
    {
        // Arrange
        var email = "user@example.com";
        var claims = new List<Claim>
        {
            new("Email", email)
        };
        SetClaimsPrincipalWithClaims(claims);

        var user = CreateUser(userId: 42, constituencyId: 5, email: email);

        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var result = await _claimsService.GetUserIdFromClaimsAsync();

        // Assert
        Assert.Equal(42, result);
        _mockUserRepository.Verify(r => r.GetUserByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task GetUserIdFromClaimsAsync_WithMissingEmailClaim_ThrowsInvalidOperationException()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new("Role", "Admin")
        };
        SetClaimsPrincipalWithClaims(claims);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _claimsService.GetUserIdFromClaimsAsync());

        // Assert
        Assert.Equal("Email claim not found.", exception.Message);
        _mockUserRepository.Verify(r => r.GetUserByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetUserIdFromClaimsAsync_WithUserNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var email = "missing@example.com";
        var claims = new List<Claim>
        {
            new("Email", email)
        };
        SetClaimsPrincipalWithClaims(claims);

        _mockUserRepository
            .Setup(r => r.GetUserByEmailAsync(email))
            .ReturnsAsync((User?)null);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _claimsService.GetUserIdFromClaimsAsync());

        // Assert
        Assert.Equal("User not found.", exception.Message);
    }

    private void SetClaimsPrincipalWithClaims(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        _httpContext.User = principal;
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(_httpContext);
    }

    private static User CreateUser(int userId, int constituencyId, string email)
    {
        return new User
        {
            UserId = userId,
            ConstituencyId = constituencyId,
            Email = email
        };
    }
}
