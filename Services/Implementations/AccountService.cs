using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Respository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using asp_net_web_api.API.Utility;

namespace asp_net_web_api.API.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private static Dictionary<string, string>? cache;
        private static object cacheLock = new object();
        public Dictionary<string,string> TokenStore
        {
            get
            {
                lock (cacheLock)
                {
                    if (cache == null)
                    {
                        cache = new Dictionary<string, string>();
                    }
                    return cache;
                }
            }
        }

        private readonly TokenStoreCache cc;

        private readonly ILogger<AccountService> _logger;

        public AccountService(IUnitOfWork unitOfWork,  IConfiguration config, ILogger<AccountService> logger, TokenStoreCache cx){
            _unitOfWork = unitOfWork;
            _config = config;
            _logger = logger;
            cc = cx;
            
        }

        public async Task<LoginResponseDto?> Login(LoginRequestDto loginReq){
            var user = await _unitOfWork.UserRepository.Login(loginReq.Name, loginReq.Password);
            if(user==null) return null;
            var loginResponse = new LoginResponseDto(){ 
                Id = user.Id,
                Name=user.Name, 
                AccessToken = CreateJWTAccessToken(user.Name, user.Id), 
                RefreshToken = CreateRefreshToken()
            };

            // TokenStore[loginResponse.AccessToken] = loginResponse.RefreshToken;
             
            if (!cc.Store.ContainsKey(loginResponse.AccessToken)){
                cc.Store.Add(loginResponse.AccessToken, loginResponse.RefreshToken);
            }
            foreach(var x in cc.Store)
            {
                _logger.LogInformation(x.Key + " " + ", "+ x.Value);
            }
            return loginResponse;
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

        private string CreateJWTAccessToken(string userName, int userId){
            var secret = _config.GetSection("AppSettings:Key").Value; 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var claims = new Claim[]{
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
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

        private string CreateRefreshToken(){
            return Guid.NewGuid().ToString();
        }

        public bool Logout(string userAccesstoken)
        {
            if(!cc.Store.ContainsKey(userAccesstoken)){
                return false;
            }
            cc.Store.Remove(userAccesstoken);
            return true;
        }

        public async Task<LoginResponseDto?> Refresh(LoginResponseDto refreshRequest)
        {
            if(cc.Store.TryGetValue(refreshRequest.AccessToken, out var storedRefreshToken))
            {
                if(refreshRequest.RefreshToken != storedRefreshToken) return null;
                
                var secret = _config.GetSection("AppSettings:Key").Value; 
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    tokenHandler.ValidateToken(refreshRequest.AccessToken, tokenValidationParameters, out _);
                }
                catch (Exception)
                {
                    return null;
                }

                var userExists = await _unitOfWork.UserRepository.UserAlreadyExists(refreshRequest.Name);
                if(!userExists) return null;
            
                var newAccessToken = CreateJWTAccessToken(refreshRequest.Name, refreshRequest.Id);
                cc.Store.Add(newAccessToken, storedRefreshToken);
                cc.Store.Remove(refreshRequest.AccessToken);

                return new LoginResponseDto(){
                    Id = refreshRequest.Id,
                    Name = refreshRequest.Name, 
                    AccessToken = newAccessToken, 
                    RefreshToken = storedRefreshToken
                };
            }

            return null;
        }
    }
}