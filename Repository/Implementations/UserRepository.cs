using System.Security.Cryptography;
using System.Text.Unicode;
using asp_net_web_api.API.Models;
using Microsoft.EntityFrameworkCore;

namespace asp_net_web_api.API.Respository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _dbContext;
        public UserRepository(AppDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<User?> Login(string userName, string userPassword){
            var user = await _dbContext.Users.FirstOrDefaultAsync( u=>u.Name==userName );
            if(user==null) return null;
            if(!MatchPasswordHash(userPassword, user.Password, user.PasswordKey)) return null;
            return user;
        }

        private static bool MatchPasswordHash(string userPassword, byte[] password, byte[] passwordKey)
        {
            using(var hmac = new HMACSHA512(passwordKey))
            {
                var userPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(userPassword));
                for(int i=0;i<userPasswordHash.Length; i++){
                    if(userPasswordHash[i]!=password[i]) return false;
                }
            }
            return true;
        }

        public void Register(string userName, string password)
        {
            byte[] passwordHash, passwordKey; 

            using(var hmac = new HMACSHA512()){
                passwordKey = hmac.Key; 
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            User user = new User(){
                Name = userName,
                Password = passwordHash, 
                PasswordKey = passwordKey, 
                CreatedAt=DateTime.UtcNow,
                ModifiedAt=DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
        }

        public async Task<bool> UserAlreadyExists(string userName)
        {
            return await _dbContext.Users.AnyAsync(u => u.Name == userName);
        }
    }
}