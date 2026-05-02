using B2B_Procurement___Order_Management_Platform.ArtMarket.Application.DTOs;
using B2B_Procurement___Order_Management_Platform.ArtMarket.Application.Services;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace B2B_Procurement___Order_Management_Platform.ArtMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO authDTO)
        {
            /// call register service --> include logic
            var result = await _authService.Register(authDTO);
            if(result.IsAuthenticated == false)
            {
                return BadRequest(result.Message);
            }
            
            return Ok(result);
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn()
        {
            return Ok();
        }

        [HttpPost("password/forgot")]
        public async Task<IActionResult> ForgotPassword()
        {
            return Ok();
        }

        [HttpPost("password/reset")]
        public async Task<IActionResult> ResetPassword()
        {
            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {
            return Ok();
        }

    }
}
