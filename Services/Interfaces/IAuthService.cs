using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;

namespace asp_net_web_api.API.Services
{
    public interface IAuthService
    {
        public bool Logout(string userAccesstoken);
        public Task<LoginResponseDto?> Login(LoginRequestDto loginRequest);
        public Task<LoginResponseDto?> Refresh(LoginResponseDto refreshRequest);
        public Task<UserDto?> Register(LoginRequestDto loginReq);
        public bool ForgotPassword(ForgotPasswordRequestDto forgotPasswordRequest);
        public Task<List<UserDto>> getUsers();
        public Task<List<Role>> getRoles();
        public Task assignUserRoles(int userId, List<int> roleIds);
        public Task removeUserRoles(int userId, List<int> roleIds);
    }
}