namespace JwtAuthentication.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService service, JwtService jwtService) : ControllerBase
{
    private readonly IUserService _service = service;
    private readonly JwtService _jwtService = jwtService;

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

        return Ok(new { Token = token });
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
}
