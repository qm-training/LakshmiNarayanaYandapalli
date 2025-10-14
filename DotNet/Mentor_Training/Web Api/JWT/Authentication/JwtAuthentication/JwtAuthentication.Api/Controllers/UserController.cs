namespace JwtAuthentication.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService service, JwtService jwtService) : ControllerBase
{
    private readonly IUserService _service = service;
    private readonly JwtService _jwtService = jwtService;

    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody] UserDto userDto)
    {
        var userPassword = await _service.GetPassword(userDto.Username);

        if (userPassword == null || userPassword != userDto.Password)
        {
            return Unauthorized("Invalid username or password");
        }

        var user = await _service.GetUserByName(userDto.Username);

        var token = _jwtService.GenerateToken(user);

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

        var user = await _service.GetUserByName(username);
        if (user == null)
        {
            return NotFound("User not found");
        }
        return Ok(user);

    }
}
