using asp_net_web_api.API.Services;
using asp_net_web_api.API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_web_api.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuthManagerController : BaseController
    { 
        private readonly IAuthService _authService;
         
        public AuthManagerController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("users/{userId}/roles")]
        public async Task<IActionResult> AssignRolesToUser(int userId, [FromBody] List<int> roleIds)
        {
            if (roleIds == null || roleIds.Count == 0){
                return BadRequest("At least one role must be provided");
            }
            await _authService.assignUserRoles(userId, roleIds);
            return Ok("Roles Assigned!");
        }

        [HttpDelete("users/{userId}/roles")]
        public async Task<IActionResult> RemoveRolesFromUser(int userId, [FromBody] List<int> roleIds)
        {
            if (roleIds == null || roleIds.Count == 0){
                return BadRequest("At least one role must be provided");
            }
            await _authService.removeUserRoles(userId, roleIds);
            return Ok("Roles Removed!");
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles(){
            var roles = await _authService.getRoles();
            return Ok(roles);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(){
            var users = await _authService.getUsers();
            return Ok(users);
        }
    }
}