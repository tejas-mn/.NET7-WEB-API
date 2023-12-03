using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_web_api.API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService AccountService;
        public AccountController(IAccountService accountService)
        {
            AccountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest){
            var authUser = await AccountService.Login(loginRequest);
            if(authUser==null) return Unauthorized();
            return Ok(authUser);
        }
    }
}