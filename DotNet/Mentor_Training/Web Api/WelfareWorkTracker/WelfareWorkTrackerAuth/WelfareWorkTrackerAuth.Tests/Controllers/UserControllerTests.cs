namespace WelfareWorkTrackerAuth.Tests.Controllers;
public class UserControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserService> _mockUserService;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        _fixture = new Fixture();
        _mockUserService = new Mock<IUserService>();
        _userController = new UserController(_mockUserService.Object);
    }

    [Fact]
    public async Task RegisterUserAsync_WithValidUser_ReturnsOkResultWithSuccessMessage()
    {
        // Arrange
        var registerUserVm = _fixture.Create<RegisterUserVm>();

        _mockUserService
            .Setup(service => service.RegisterAsync(registerUserVm))
            .ReturnsAsync(true);

        // Act
        var result = await _userController.RegisterUserAsync(registerUserVm);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("User registered Successfully", okResult.Value);
    }

    [Fact]
    public async Task RegisterUserAsync_WithInvalidUser_ReturnsBadRequestWithFailureMessage()
    {
        // Arrange
        var registerUserVm = _fixture.Create<RegisterUserVm>();

        _mockUserService
            .Setup(service => service.RegisterAsync(registerUserVm))
            .ReturnsAsync(false);

        // Act
        var result = await _userController.RegisterUserAsync(registerUserVm);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Unable to register user", badRequestResult.Value);
    }

    [Fact]
    public async Task LoginUserAsync_WithValidCredentials_ReturnsOkResultWithTokens()
    {
        // Arrange
        var loginUserVm = _fixture.Create<LoginUserVm>();
        var expectedTokens = _fixture.Create<LoginUserDto>();

        _mockUserService
            .Setup(service => service.LoginAsync(loginUserVm))
            .ReturnsAsync(expectedTokens);

        // Act
        var result = await _userController.LoginUserAsync(loginUserVm);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<LoginUserDto>(okResult.Value);
        Assert.Equal(expectedTokens, value);
    }

    [Fact]
    public async Task ResetPasswordAsync_WithValidData_ReturnsOkResultWithSuccessMessage()
    {
        // Arrange
        var resetPasswordVm = _fixture.Create<ResetPasswordVm>();

        _mockUserService
            .Setup(service => service.ResetPasswordAsync(resetPasswordVm))
            .ReturnsAsync(true);

        // Act
        var result = await _userController.ResetPasswordAsync(resetPasswordVm);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("password reset Successful", okResult.Value);
    }

    [Fact]
    public async Task ResetPasswordAsync_WithInvalidData_ReturnsBadRequestWithFailureMessage()
    {
        // Arrange
        var resetPasswordVm = _fixture.Create<ResetPasswordVm>();

        _mockUserService
            .Setup(service => service.ResetPasswordAsync(resetPasswordVm))
            .ReturnsAsync(false);

        // Act
        var result = await _userController.ResetPasswordAsync(resetPasswordVm);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Unable to reset password", badRequestResult.Value);
    }

    [Fact]
    public async Task RenewAccessTokenAsync_WithValidInput_ReturnsOkResultWithNewTokens()
    {
        // Arrange
        var renewAccessTokenVm = _fixture.Create<RenewAccessTokenVm>();
        var expectedTokens = _fixture.Create<LoginUserDto>();

        _mockUserService
            .Setup(service => service.RenewAccessTokenAsync(renewAccessTokenVm.Email, renewAccessTokenVm.RefreshToken))
            .ReturnsAsync(expectedTokens);

        // Act
        var result = await _userController.RenewAccessTokenAsync(renewAccessTokenVm);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<LoginUserDto>(okResult.Value);
        Assert.Equal(expectedTokens, value);
    }
}
