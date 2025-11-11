namespace WelfareWorkTrackerAuth.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUserAsync(RegisterUserVm registerUserVm)
    {
        var result = await _userService.RegisterAsync(registerUserVm);
        if (!result)
            return BadRequest("Unable to register user");

        return Ok("User registered Successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUserAsync(LoginUserVm loginUserVm)
    {
        var tokens = await _userService.LoginAsync(loginUserVm);
        return Ok(tokens);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordVm resetPasswordVm)
    {
        var result = await _userService.ResetPasswordAsync(resetPasswordVm);
        if (!result)
            return BadRequest("Unable to reset password");

        return Ok("password reset Successful");
    }

    [HttpPost("renew-access-token")]
    public async Task<IActionResult> RenewAccessTokenAsync(RenewAccessTokenVm renewAccessTokenVm)
    {
        var result = await _userService.RenewAccessTokenAsync(renewAccessTokenVm.Email, renewAccessTokenVm.RefreshToken);
        return Ok(result);
    }
}