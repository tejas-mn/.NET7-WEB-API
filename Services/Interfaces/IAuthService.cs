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
        public Task<bool> ForgotPassword(ForgotPasswordRequestDto forgotPasswordRequest);
    }
}