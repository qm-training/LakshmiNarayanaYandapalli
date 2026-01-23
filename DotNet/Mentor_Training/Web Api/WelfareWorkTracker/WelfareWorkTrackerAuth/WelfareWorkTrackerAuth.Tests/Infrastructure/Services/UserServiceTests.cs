namespace WelfareWorkTrackerAuth.Tests.Infrastructure.Services;
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IConstituencyRepository> _mockConstituencyRepository;
    private readonly Mock<IEmailTemplateRepository> _mockEmailTemplateRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockConstituencyRepository = new Mock<IConstituencyRepository>();
        _mockEmailTemplateRepository = new Mock<IEmailTemplateRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockAuthService = new Mock<IAuthService>();

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockConstituencyRepository.Object,
            _mockEmailTemplateRepository.Object,
            _mockEmailService.Object,
            _mockAuthService.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsLoginUserDtoAndUpdatesUser()
    {
        // Arrange
        var loginUserVm = new LoginUserVm
        {
            Email = "test@example.com",
            Password = "Password123"
        };

        var user = CreateUser(loginUserVm.Email);

        _mockUserRepository
            .Setup(r => r.CheckIfUserEmailExistsAsync(loginUserVm.Email))
            .ReturnsAsync(user);

        _mockAuthService
            .Setup(a => a.VerifyPassword(loginUserVm.Password, user.PasswordSalt, user.PasswordHash))
            .Returns(true);

        const string accessToken = "access-token";
        const string refreshToken = "refresh-token";

        _mockAuthService
            .Setup(a => a.GenerateJwtToken(user))
            .Returns(accessToken);

        _mockAuthService
            .Setup(a => a.GenerateRefreshToken())
            .Returns(refreshToken);

        _mockUserRepository
            .Setup(r => r.UpdateUserAsync(user))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.LoginAsync(loginUserVm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accessToken, result.Accesstoken);
        Assert.Equal(refreshToken, result.RefreshToken);

        _mockUserRepository.Verify(r => r.CheckIfUserEmailExistsAsync(loginUserVm.Email), Times.Once);
        _mockAuthService.Verify(a => a.VerifyPassword(loginUserVm.Password, user.PasswordSalt, user.PasswordHash), Times.Once);
        _mockAuthService.Verify(a => a.GenerateJwtToken(user), Times.Once);
        _mockAuthService.Verify(a => a.GenerateRefreshToken(), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithNonExistingEmail_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var loginUserVm = new LoginUserVm
        {
            Email = "notfound@example.com",
            Password = "Password123"
        };

        _mockUserRepository
            .Setup(r => r.CheckIfUserEmailExistsAsync(loginUserVm.Email))
            .ReturnsAsync((User?)null);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _userService.LoginAsync(loginUserVm));

        // Assert
        Assert.Equal("Invalid email or password.", exception.Message);
        _mockAuthService.Verify(a => a.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithIncorrectPassword_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var loginUserVm = new LoginUserVm
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        var user = CreateUser(loginUserVm.Email);

        _mockUserRepository
            .Setup(r => r.CheckIfUserEmailExistsAsync(loginUserVm.Email))
            .ReturnsAsync(user);

        _mockAuthService
            .Setup(a => a.VerifyPassword(loginUserVm.Password, user.PasswordSalt, user.PasswordHash))
            .Returns(false);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _userService.LoginAsync(loginUserVm));

        // Assert
        Assert.Equal("Invalid email or password.", exception.Message);
        _mockAuthService.Verify(a => a.GenerateJwtToken(It.IsAny<User>()), Times.Never);
        _mockAuthService.Verify(a => a.GenerateRefreshToken(), Times.Never);
        _mockUserRepository.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_RegistersUserAndSendsWelcomeEmail()
    {
        // Arrange
        var registerUserVm = new RegisterUserVm
        {
            FullName = "Test User",
            Email = "test@example.com",
            Age = 30,
            MobileNumber = 1234567890,
            Gender = "Male",
            Address = "Address",
            RoleName = Roles.Citizen.ToString(),
            ConstituencyName = "SomeConstituency",
            Password = "Password123"
        };

        const string salt = "salt";
        const string hash = "hash";
        const int constituencyId = 5;
        var emailTemplate = new EmailTemplate
        {
            Id = 10,
            Name = "WelcomeEmail"
        };

        _mockAuthService
            .Setup(a => a.GenerateSalt())
            .Returns(salt);

        _mockAuthService
            .Setup(a => a.GeneratePasswordHash(registerUserVm.Password, salt))
            .Returns(hash);

        _mockConstituencyRepository
            .Setup(c => c.GetConstituencyIdByNameAsync(registerUserVm.ConstituencyName))
            .ReturnsAsync(constituencyId);

        _mockUserRepository
            .Setup(r => r.RegisterUserAsync(It.IsAny<User>()))
            .ReturnsAsync(true);

        _mockEmailTemplateRepository
            .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(emailTemplate);

        _mockEmailService
            .Setup(e => e.SendEmailAsync(It.IsAny<EmailVm>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.RegisterAsync(registerUserVm);

        // Assert
        Assert.True(result);

        _mockAuthService.Verify(a => a.GenerateSalt(), Times.Once);
        _mockAuthService.Verify(a => a.GeneratePasswordHash(registerUserVm.Password, salt), Times.Once);
        _mockConstituencyRepository.Verify(c => c.GetConstituencyIdByNameAsync(registerUserVm.ConstituencyName), Times.Once);
        _mockUserRepository.Verify(r => r.RegisterUserAsync(It.IsAny<User>()), Times.Once);
        _mockEmailTemplateRepository.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Once);
        _mockEmailService.Verify(e => e.SendEmailAsync(It.Is<EmailVm>(vm =>
            vm.TemplateId == emailTemplate.Id &&
            vm.ToUserEmail == registerUserVm.Email &&
            vm.Payload["FullName"] == registerUserVm.FullName &&
            vm.Payload["Email"] == registerUserVm.Email)), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenUserNotRegistered_DoesNotSendWelcomeEmailAndReturnsTrue()
    {
        // Arrange
        var registerUserVm = new RegisterUserVm
        {
            FullName = "Test User",
            Email = "test@example.com",
            Age = 30,
            MobileNumber = 1234567890,
            Gender = "Male",
            Address = "Address",
            RoleName = Roles.Citizen.ToString(),
            ConstituencyName = "SomeConstituency",
            Password = "Password123"
        };

        const string salt = "salt";
        const string hash = "hash";
        const int constituencyId = 5;
        var emailTemplate = new EmailTemplate
        {
            Id = 10,
            Name = "WelcomeEmail"
        };

        _mockAuthService
            .Setup(a => a.GenerateSalt())
            .Returns(salt);

        _mockAuthService
            .Setup(a => a.GeneratePasswordHash(registerUserVm.Password, salt))
            .Returns(hash);

        _mockConstituencyRepository
            .Setup(c => c.GetConstituencyIdByNameAsync(registerUserVm.ConstituencyName))
            .ReturnsAsync(constituencyId);

        _mockUserRepository
            .Setup(r => r.RegisterUserAsync(It.IsAny<User>()))
            .ReturnsAsync(false);

        _mockEmailTemplateRepository
            .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(emailTemplate);

        // Act
        var result = await _userService.RegisterAsync(registerUserVm);

        // Assert
        Assert.True(result);

        _mockEmailService.Verify(e => e.SendEmailAsync(It.IsAny<EmailVm>()), Times.Never);
    }

    [Fact]
    public async Task RenewAccessTokenAsync_WithValidRefreshToken_ReturnsNewTokensAndUpdatesUser()
    {
        // Arrange
        const string email = "test@example.com";
        const string currentRefreshToken = "refresh-token";

        var user = CreateUser(email);
        user.RefreshToken = currentRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(10);

        _mockUserRepository
            .Setup(r => r.CheckIfUserEmailExistsAsync(email))
            .ReturnsAsync(user);

        const string newAccessToken = "new-access-token";
        const string newRefreshToken = "new-refresh-token";

        _mockAuthService
            .Setup(a => a.GenerateJwtToken(user))
            .Returns(newAccessToken);

        _mockAuthService
            .Setup(a => a.GenerateRefreshToken())
            .Returns(newRefreshToken);

        _mockUserRepository
            .Setup(r => r.UpdateUserAsync(user))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.RenewAccessTokenAsync(email, currentRefreshToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newAccessToken, result.Accesstoken);
        Assert.Equal(newRefreshToken, result.RefreshToken);

        _mockUserRepository.Verify(r => r.CheckIfUserEmailExistsAsync(email), Times.Once);
        _mockAuthService.Verify(a => a.GenerateJwtToken(user), Times.Once);
        _mockAuthService.Verify(a => a.GenerateRefreshToken(), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task RenewAccessTokenAsync_WithInvalidEmail_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        const string email = "invalid@example.com";
        const string refreshToken = "refresh-token";

        _mockUserRepository
            .Setup(r => r.CheckIfUserEmailExistsAsync(email))
            .ReturnsAsync((User?)null);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _userService.RenewAccessTokenAsync(email, refreshToken));

        // Assert
        Assert.Equal("Invalid email.", exception.Message);
        _mockAuthService.Verify(a => a.GenerateJwtToken(It.IsAny<User>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RenewAccessTokenAsync_WithInvalidRefreshToken_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        const string email = "test@example.com";
        const string providedRefreshToken = "provided-token";

        var user = CreateUser(email);
        user.RefreshToken = "different-token";
        user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(10);

        _mockUserRepository
            .Setup(r => r.CheckIfUserEmailExistsAsync(email))
            .ReturnsAsync(user);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _userService.RenewAccessTokenAsync(email, providedRefreshToken));

        // Assert
        Assert.Equal("Refresh not valid.", exception.Message);
        _mockAuthService.Verify(a => a.GenerateJwtToken(It.IsAny<User>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ResetPasswordAsync_WithValidData_UpdatesPasswordAndSendsEmail()
    {
        // Arrange
        var resetPasswordVm = new ResetPasswordVm
        {
            Email = "test@example.com",
            OldPassword = "OldPassword",
            NewPassword = "NewPassword"
        };

        var user = CreateUser(resetPasswordVm.Email);

        _mockUserRepository
            .Setup(r => r.CheckIfUserEmailExistsAsync(resetPasswordVm.Email))
            .ReturnsAsync(user);

        _mockAuthService
            .Setup(a => a.VerifyPassword(resetPasswordVm.OldPassword, user.PasswordSalt, user.PasswordHash))
            .Returns(true);

        const string newSalt = "new-salt";
        const string newHash = "new-hash";

        _mockAuthService
            .Setup(a => a.GenerateSalt())
            .Returns(newSalt);

        _mockAuthService
            .Setup(a => a.GeneratePasswordHash(resetPasswordVm.NewPassword, newSalt))
            .Returns(newHash);

        _mockUserRepository
            .Setup(r => r.UpdateUserAsync(user))
            .ReturnsAsync(true);

        var emailTemplate = new EmailTemplate
        {
            Id = 20,
            Name = "PasswordResetSuccessful"
        };

        _mockEmailTemplateRepository
            .Setup(r => r.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(emailTemplate);

        _mockEmailService
            .Setup(e => e.SendEmailAsync(It.IsAny<EmailVm>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.ResetPasswordAsync(resetPasswordVm);

        // Assert
        Assert.True(result);

        Assert.Equal(newHash, user.PasswordHash);
        Assert.Equal(newSalt, user.PasswordSalt);

        _mockUserRepository.Verify(r => r.UpdateUserAsync(user), Times.Once);
        _mockEmailTemplateRepository.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Once);
        _mockEmailService.Verify(e => e.SendEmailAsync(It.Is<EmailVm>(vm =>
            vm.TemplateId == emailTemplate.Id &&
            vm.ToUserEmail == resetPasswordVm.Email &&
            vm.Payload["FullName"] == user.FullName &&
            vm.Payload["Email"] == user.Email)), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_WithInvalidEmail_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var resetPasswordVm = new ResetPasswordVm
        {
            Email = "invalid@example.com",
            OldPassword = "OldPassword",
            NewPassword = "NewPassword"
        };

        _mockUserRepository
            .Setup(r => r.CheckIfUserEmailExistsAsync(resetPasswordVm.Email))
            .ReturnsAsync((User?)null);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _userService.ResetPasswordAsync(resetPasswordVm));

        // Assert
        Assert.Equal("Invalid email.", exception.Message);
        _mockAuthService.Verify(a => a.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ResetPasswordAsync_WithIncorrectOldPassword_ThrowsWelfareWorkTrackerException()
    {
        // Arrange
        var resetPasswordVm = new ResetPasswordVm
        {
            Email = "test@example.com",
            OldPassword = "WrongPassword",
            NewPassword = "NewPassword"
        };

        var user = CreateUser(resetPasswordVm.Email);

        _mockUserRepository
            .Setup(r => r.CheckIfUserEmailExistsAsync(resetPasswordVm.Email))
            .ReturnsAsync(user);

        _mockAuthService
            .Setup(a => a.VerifyPassword(resetPasswordVm.OldPassword, user.PasswordSalt, user.PasswordHash))
            .Returns(false);

        // Act
        var exception = await Assert.ThrowsAsync<WelfareWorkTrackerException>(() => _userService.ResetPasswordAsync(resetPasswordVm));

        // Assert
        Assert.Equal("Incorrect password.", exception.Message);
        _mockUserRepository.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        _mockEmailService.Verify(e => e.SendEmailAsync(It.IsAny<EmailVm>()), Times.Never);
    }

    private static User CreateUser(string email)
    {
        return new User
        {
            UserId = 1,
            FullName = "Test User",
            Email = email,
            Age = 30,
            MobileNumber = 1234567890,
            Gender = "Male",
            Address = "Address",
            ConstituencyId = 1,
            PasswordSalt = "salt",
            PasswordHash = "hash",
            RoleId = (int)Roles.Citizen,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };
    }
}
