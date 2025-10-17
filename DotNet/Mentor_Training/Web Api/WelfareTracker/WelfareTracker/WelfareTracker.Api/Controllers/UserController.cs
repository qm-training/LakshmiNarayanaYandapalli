using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserVm registerUserVm)
        {
            var result = await _userService.AddUserAsync(registerUserVm);
            return Ok(result);
        }

        [HttpPost("user-login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserVm loginUserVm)
        {
            var result = await _userService.LoginUser(loginUserVm);
            if (result == null)
            {
                return Unauthorized("Invalid email or password.");
            }
            return Ok(result);
        }
    }
}
