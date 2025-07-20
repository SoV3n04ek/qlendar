using Microsoft.AspNetCore.Mvc;
using QlendarBackend.Qlendar.API;

namespace QlendarBackend.Qlendar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase 
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(
                request.Email,
                request.Password,
                request.Nickname
            );

            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { token = result.Token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request.Email,
                                                       request.Password);

            if (!result.Success)
                return Unauthorized(new { errors = result.Errors });

            return Ok(new { token = result.Token });
        }
    }
}
