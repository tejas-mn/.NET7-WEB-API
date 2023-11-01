using asp_net_web_api.API.Models;

namespace asp_net_web_api.API.Respository
{
    public class ItemRepository : GenericRepository<InventoryItem>, IItemRepository
    {
        public ItemRepository(AppDbContext context) : base(context){

        }
    }
}