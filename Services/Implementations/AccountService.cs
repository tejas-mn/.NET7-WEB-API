using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Respository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace asp_net_web_api.API.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public AccountService(IUnitOfWork unitOfWork,  IConfiguration config){
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<LoginResponseDto?> Login(LoginRequestDto loginReq){
            var user = await _unitOfWork.UserRepository.Login(loginReq.Name, loginReq.Password);
            if(user==null) return null;
            return new LoginResponseDto(){ Name=user.Name, Token = CreateJWT(user)};
        }

        public async Task<UserDto?> Register(LoginRequestDto loginReq){
            var user = await _unitOfWork.UserRepository.UserAlreadyExists(loginReq.Name);
            if(user==true) return null;
            
            _unitOfWork.UserRepository.Register(loginReq.Name, loginReq.Password);
            _unitOfWork.Complete();
            
            return new UserDto(){
                Name = loginReq.Name, 
                CreatedAt = DateTime.UtcNow
            };
        }

        private string CreateJWT(User user){
            var secret = _config.GetSection("AppSettings:Key").Value; 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var claims = new Claim[]{
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var signingCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(2),
                SigningCredentials = signingCred
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}