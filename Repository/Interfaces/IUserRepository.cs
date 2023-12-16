using asp_net_web_api.API.Respository;
using asp_net_web_api.API.Models;

namespace asp_net_web_api.API.Respository {
    public interface IUserRepository : IGenericRepository<User>
    {
        public Task<User?> Login(string userName, string userPassword);
        public void Register(string userName, string password);
        public Task<bool> UserAlreadyExists(string userName);
    }
}