using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_web_api.API.Controllers
{
    public class AuthController : BaseController
    { 
        private readonly IAuthService _authService;
         
        public AuthController(IAuthService authService){
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            
            var authUser = await _authService.Login(loginRequest);

            if(authUser==null) return Unauthorized("Invalid credentials");
            
            CookieOptions cookieOptions = new CookieOptions {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.Now.AddMinutes(15) 
            };

            HttpContext.Response.Cookies.Append("access_token", authUser.AccessToken, cookieOptions);
            HttpContext.Response.Cookies.Append("refresh_token", authUser.RefreshToken, cookieOptions);
            
            return Ok(authUser);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(LoginRequestDto registerRequest)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _authService.Register(registerRequest);
            if(user == null) return BadRequest("User already exists");
            return Ok(user);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var userAccesstoken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var loggedOut = _authService.Logout(userAccesstoken);
            if(!loggedOut) return BadRequest("Already Logged Out");
            return Ok("Logged out succesfully!");
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] LoginResponseDto refreshRequest)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var response = await _authService.Refresh(refreshRequest);
            if(response == null) return BadRequest("Error while refreshing! AccessToken could be expired");
            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequest)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var reset =  await _authService.ForgotPassword(forgotPasswordRequest);
            if(!reset) return BadRequest("Error while updating new password");
            return Ok("Password Reset Successfull!");
        }
    }
}