using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Respository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using asp_net_web_api.API.Utility;
using System.Security.Cryptography;

namespace asp_net_web_api.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly TokenStoreCache _tokenStore;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUnitOfWork unitOfWork,  IConfiguration config, ILogger<AuthService> logger, TokenStoreCache tokenStore){
            _unitOfWork = unitOfWork;
            _config = config;
            _logger = logger;
            _tokenStore = tokenStore;
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
            if (!_tokenStore.Store.ContainsKey(loginResponse.AccessToken)){
                _tokenStore.Store.Add(loginResponse.AccessToken, loginResponse.RefreshToken);
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

        public async Task<bool> ForgotPassword(ForgotPasswordRequestDto forgotPasswordRequest){
            // var user = await _unitOfWork.UserRepository.UserAlreadyExists(forgotPasswordRequest.Name);
            var userr = _unitOfWork.UserRepository.Find(u => u.Name == forgotPasswordRequest.Name).FirstOrDefault();
            if(userr==null) throw new Exception("User Not Found");
            
            byte[] passwordHash, passwordKey; 

            using(var hmac = new HMACSHA512()){
                passwordKey = hmac.Key; 
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(forgotPasswordRequest.NewPassword));
            }

            userr.Password = passwordHash;
            userr.PasswordKey = passwordKey;

            _unitOfWork.UserRepository.Update(userr);
            _unitOfWork.Complete();

            return true;
        }

        private string CreateJWTAccessToken(string userName, int userId){
            var secret = _config.GetSection("AppSettings:Key").Value;
           
            //userRoles
            List<string> userRoles = _unitOfWork.UserRepository.GetUserRolesByUserId(userId);

            //rolePermssions
            HashSet<string> permissions = new HashSet<string>();
            foreach(var role in userRoles){
                _logger.LogInformation("Role: " + role);
                var roleId = _unitOfWork.UserRepository.GetRoleIdByName(role);
                var perms = _unitOfWork.UserRepository.GetRolePermissionsByRoleId(roleId);
                foreach(var perm in perms) permissions.Add(perm);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
              
               //role & permissions
            };
            
            // Add roles to claims
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.AddRange(permissions.Select(perm => new Claim("Permission", perm)));
            

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
            if(!_tokenStore.Store.ContainsKey(userAccesstoken)){
                return false;
            }
            _tokenStore.Store.Remove(userAccesstoken);
            return true;
        }

        public async Task<LoginResponseDto?> Refresh(LoginResponseDto refreshRequest)
        {
            if(_tokenStore.Store.TryGetValue(refreshRequest.AccessToken, out var storedRefreshToken))
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

                try{
                    var tokenHandler = new JwtSecurityTokenHandler();
                    tokenHandler.ValidateToken(refreshRequest.AccessToken, tokenValidationParameters, out _);
                }
                catch (Exception){
                    return null;
                }

                var userExists = await _unitOfWork.UserRepository.UserAlreadyExists(refreshRequest.Name);
                if(!userExists) return null;
            
                var newAccessToken = CreateJWTAccessToken(refreshRequest.Name, refreshRequest.Id);
                _tokenStore.Store.Add(newAccessToken, storedRefreshToken);
                _tokenStore.Store.Remove(refreshRequest.AccessToken);
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