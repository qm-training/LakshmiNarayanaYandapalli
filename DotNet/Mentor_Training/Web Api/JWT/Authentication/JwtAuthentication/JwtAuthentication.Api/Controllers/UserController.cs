namespace JwtAuthentication.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService service, JwtService jwtService, IRefreshTokenService refreshTokenService) : ControllerBase
{
    private readonly IUserService _service = service;
    private readonly JwtService _jwtService = jwtService;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;

    [HttpGet("/login")]
    public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
    {
        if(!await _service.PasswordCheckAsync(username, password))
        {
            return Unauthorized("Invalid username or password");
        }

        var user = await _service.GetUserByNameAsync(username);

        if (user == null) {
            return NotFound("User not found");
        }

        var token = _jwtService.GenerateToken(user!);
        var refreshToken = await _refreshTokenService.AddRefreshTokenAsync(username);

        return Ok(new { Token = token, RefreshToken = refreshToken });
    }

    [Authorize]
    [HttpGet("/user")]
    public async Task<IActionResult> GetUser()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized("User is not authenticated");
        }

        var user = await _service.GetUserByNameAsync(username);
        if (user == null)
        {
            return NotFound("User not found");
        }
        return Ok(user);

    }

    [HttpPost("/register")]
    public async Task<IActionResult> Register([FromBody] UserDto userDto)
    {
        var result = await _service.AddUserAsync(userDto);
        return Ok(result);
    }

    [HttpGet("/renew")]
    public async Task<IActionResult> RenewToken([FromQuery] string username, [FromQuery] string refreshToken)
    {
        var isValid = await _refreshTokenService.ValidateRefreshTokenAsync(username, refreshToken);
        if (!isValid)
        {
            return Unauthorized("Invalid refresh token");
        }
        var user = await _service.GetUserByNameAsync(username);
        if (user == null)
        {
            return NotFound("User not found");
        }
        var newToken = _jwtService.GenerateToken(user);
        var newRefreshToken = await _refreshTokenService.RenewRefreshTokenAsync(username, refreshToken);
        return Ok(new { Token = newToken, RefreshToken = newRefreshToken });
    }
}
