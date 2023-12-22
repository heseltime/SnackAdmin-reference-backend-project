using Microsoft.AspNetCore.Mvc;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Services;
using SnackAdmin.Domain;
using SnackAdmin.Dtos;

namespace SnackAdmin.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _jwtTokenService;
        private readonly IAuthManagementLogic _logic;

        public AuthController(JwtTokenService jwtTokenService, IAuthManagementLogic logic)
        {
            _jwtTokenService = jwtTokenService;
            _logic = logic;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var credentialsCheckPassed = await _logic.IsValidUser(login);

            if (credentialsCheckPassed)
            {
                var token = _jwtTokenService.GenerateToken(login.RestaurantName);
                return Ok(new { token });
            }

            return Unauthorized();
        }

    }
}
