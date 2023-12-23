using asp_net_web_api.API.Models;
using Microsoft.EntityFrameworkCore;

namespace asp_net_web_api.API.Respository
{
    public class ItemRepository : GenericRepository<Product>, IItemRepository
    {
        public ItemRepository(AppDbContext context) : base(context) {
            
        }
    }
}