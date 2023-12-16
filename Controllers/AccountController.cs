using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Services;
using asp_net_web_api.API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_web_api.API.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAccountService _accountService;
      
        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var authUser = await _accountService.Login(loginRequest);
            if(authUser==null) return Unauthorized("Invalid credentials");
            return Ok(authUser);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(LoginRequestDto loginRequest)
        {
            var user = await _accountService.Register(loginRequest);
            if(user == null) return BadRequest("User already exists");
            return Ok(user);
        }

        // [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userAccesstoken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var loggedOut = _accountService.Logout(userAccesstoken);
            if(!loggedOut) return BadRequest("Already Logged Out");
            return Ok("Logged out succesfully!");
        }

        // [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] LoginResponseDto refreshRequest)
        {
            var response = await _accountService.Refresh(refreshRequest);
            if(response == null) return BadRequest("Error while refreshing! AccessToken coulb be expired");
            return Ok(response);
        }
    }
}