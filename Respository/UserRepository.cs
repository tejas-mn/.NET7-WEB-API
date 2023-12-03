using asp_net_web_api.API.Models;

namespace asp_net_web_api.API.Respository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }
    }
}