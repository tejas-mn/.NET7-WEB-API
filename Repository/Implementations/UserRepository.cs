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
    
        public List<string> GetUserRolesByUserId(int userId){
            List<int> roleIds = _dbContext.UserRoles.Where(t => t.UserId == userId).Select(t => t.RoleId).AsEnumerable<int>().ToList<int>();
            List<string> roles = new List<string>(); 
            foreach(var id in roleIds){
                roles.Add(_dbContext.Roles.Where(t => t.Id == id).Select(t => t.Name).First());
            }
            return roles;
        }

        public List<string> GetRolePermissionsByRoleId(int roleId){
            List<int> permissionIds = _dbContext.RolePermissions.Where(t => t.RoleId == roleId).Select(t => t.PermissionId).AsEnumerable<int>().ToList<int>();
            List<string> permissions = new List<string>(); 
            foreach(var id in permissionIds){
                permissions.Add(_dbContext.Permissions.Where(t => t.Id == id).Select(t => t.Name).First());
            }
            return permissions;
        }

        public int GetRoleIdByName(string role){
            return _dbContext.Roles.Where(t => t.Name == role).Select(t=>t.Id).First();
        }

        public async Task<List<Role>> GetAllRoles(){
          return await _dbContext.Roles.ToListAsync();
        }

        public async Task AssignUserRoles(int userId, List<int> roleIds){
            foreach(var roleId in roleIds){
                var t = await _dbContext.UserRoles.FindAsync(userId,roleId);
                if(t!=null) throw new Exception($"Role {roleId} is already assigned to user {userId}");
                await _dbContext.UserRoles.AddAsync(new UserRole{RoleId = roleId, UserId = userId});
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveUserRoles(int userId, List<int> roleIds){
            foreach(var roleId in roleIds){
                var t = await _dbContext.UserRoles.FindAsync(userId,roleId);
                _dbContext.UserRoles.Remove(t);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}