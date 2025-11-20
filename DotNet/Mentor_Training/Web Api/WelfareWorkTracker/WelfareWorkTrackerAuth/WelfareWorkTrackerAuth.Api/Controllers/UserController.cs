namespace WelfareWorkTrackerAuth.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="registerUserVm">The user details required for registration.</param>
    /// <remarks>
    /// This endpoint creates a new user account with the provided credentials and profile information.
    /// Returns a failure response if the registration process is unsuccessful.
    /// </remarks>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating whether the registration was successful.
    /// </returns>
    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUserAsync(RegisterUserVm registerUserVm)
    {
        var result = await _userService.RegisterAsync(registerUserVm);
        if (!result)
            return BadRequest("Unable to register user");

        return Ok("User registered Successfully");
    }

    /// <summary>
    /// Authenticates a user and generates authentication tokens.
    /// </summary>
    /// <param name="loginUserVm">The user's login credentials.</param>
    /// <remarks>
    /// This endpoint validates the provided credentials and returns access/refresh tokens on success.
    /// </remarks>
    /// <returns>
    /// An <see cref="IActionResult"/> containing authentication tokens.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> LoginUserAsync(LoginUserVm loginUserVm)
    {
        var tokens = await _userService.LoginAsync(loginUserVm);
        return Ok(tokens);
    }

    /// <summary>
    /// Resets the password of an existing user.
    /// </summary>
    /// <param name="resetPasswordVm">The required information to reset the user’s password.</param>
    /// <remarks>
    /// This endpoint verifies the user's identity and updates the password.
    /// Returns a failure response if verification or update fails.
    /// </remarks>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating the password reset status.
    /// </returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordVm resetPasswordVm)
    {
        var result = await _userService.ResetPasswordAsync(resetPasswordVm);
        if (!result)
            return BadRequest("Unable to reset password");

        return Ok("password reset Successful");
    }

    /// <summary>
    /// Renews an expired or soon-to-expire access token using a valid refresh token.
    /// </summary>
    /// <param name="renewAccessTokenVm">Contains the email and refresh token of the user.</param>
    /// <remarks>
    /// This endpoint generates a new access token if the provided refresh token is valid.
    /// </remarks>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the new access token.
    /// </returns>
    [HttpPost("renew-access-token")]
    public async Task<IActionResult> RenewAccessTokenAsync(RenewAccessTokenVm renewAccessTokenVm)
    {
        var result = await _userService.RenewAccessTokenAsync(renewAccessTokenVm.Email, renewAccessTokenVm.RefreshToken);
        return Ok(result);
    }
}