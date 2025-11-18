namespace WelfareWorkTrackerAuth.Tests.Infrastructure.Services;
public class AuthServiceTests
{
    private readonly Mock<IOptions<JwtOptions>> _mockOptions;
    private readonly JwtOptions _jwtOptions;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _jwtOptions = new JwtOptions
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            Key = "0123456789ABCDEF0123456789ABCDEF",
            ExpiryMinutes = 60
        };

        _mockOptions = new Mock<IOptions<JwtOptions>>();
        _mockOptions.Setup(o => o.Value).Returns(_jwtOptions);

        _authService = new AuthService(_mockOptions.Object);
    }

    [Fact]
    public void GenerateJwtToken_WithValidUser_ReturnsValidTokenString()
    {
        // Arrange
        var user = CreateUserWithValidRole();

        // Act
        var token = _authService.GenerateJwtToken(user);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal(_jwtOptions.Issuer, jwtToken.Issuer);
        Assert.Equal(_jwtOptions.Audience, jwtToken.Audiences.Single());
        Assert.Contains(jwtToken.Claims, c => c.Type == "Email" && c.Value == user.Email);
        Assert.Contains(jwtToken.Claims, c => c.Type == "Id" && c.Value == user.UserId.ToString());
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role);
    }

    [Fact]
    public void GenerateJwtToken_WithNullEmail_SetsEmailClaimToEmptyString()
    {
        // Arrange
        var user = CreateUserWithValidRole();
        user.Email = null!;

        // Act
        var token = _authService.GenerateJwtToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var emailClaim = jwtToken.Claims.Single(c => c.Type == "Email");
        Assert.Equal(string.Empty, emailClaim.Value);
    }

    [Fact]
    public void GeneratePasswordHash_WithSamePasswordAndSalt_ProducesSameHash()
    {
        // Arrange
        var password = "TestPassword123!";
        var salt = "TestSaltValue";

        // Act
        var hash1 = _authService.GeneratePasswordHash(password, salt);
        var hash2 = _authService.GeneratePasswordHash(password, salt);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GeneratePasswordHash_WithDifferentPasswordOrSalt_ProducesDifferentHash()
    {
        // Arrange
        var password = "TestPassword123!";
        var differentPassword = "OtherPassword456!";
        var salt = "TestSaltValue";
        var differentSalt = "DifferentSaltValue";

        // Act
        var hash1 = _authService.GeneratePasswordHash(password, salt);
        var hash2 = _authService.GeneratePasswordHash(differentPassword, salt);
        var hash3 = _authService.GeneratePasswordHash(password, differentSalt);

        // Assert
        Assert.NotEqual(hash1, hash2);
        Assert.NotEqual(hash1, hash3);
    }

    [Fact]
    public void GenerateRefreshToken_WhenCalled_ReturnsNonEmptyBase64String()
    {
        // Arrange

        // Act
        var refreshToken = _authService.GenerateRefreshToken();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(refreshToken));
        Convert.FromBase64String(refreshToken);
    }

    [Fact]
    public void GenerateRefreshToken_WhenCalledTwice_ReturnsDifferentTokens()
    {
        // Arrange

        // Act
        var token1 = _authService.GenerateRefreshToken();
        var token2 = _authService.GenerateRefreshToken();

        // Assert
        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void GenerateSalt_WhenCalled_ReturnsNonEmptyString()
    {
        // Arrange

        // Act
        var salt = _authService.GenerateSalt();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(salt));
        Convert.FromBase64String(salt);
    }

    [Fact]
    public void GenerateSalt_WhenCalledTwice_ReturnsDifferentSalts()
    {
        // Arrange

        // Act
        var salt1 = _authService.GenerateSalt();
        var salt2 = _authService.GenerateSalt();

        // Assert
        Assert.NotEqual(salt1, salt2);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "TestPassword123!";
        var salt = _authService.GenerateSalt();
        var passwordHash = _authService.GeneratePasswordHash(password, salt);

        // Act
        var result = _authService.VerifyPassword(password, salt, passwordHash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var incorrectPassword = "WrongPassword!";
        var salt = _authService.GenerateSalt();
        var passwordHash = _authService.GeneratePasswordHash(password, salt);

        // Act
        var result = _authService.VerifyPassword(incorrectPassword, salt, passwordHash);

        // Assert
        Assert.False(result);
    }

    private static User CreateUserWithValidRole()
    {
        return new User
        {
            UserId = 1,
            Email = "testuser@example.com",
            RoleId = (int)Roles.Admin
        };
    }
}
