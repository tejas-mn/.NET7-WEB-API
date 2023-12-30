using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;

namespace asp_net_web_api.API.Services
{
    public interface IProductService
    {
         public Task<List<ItemDto>> getInventoryItems(ProductQueryParameters queryParameters);
         public ItemDto? getInventoryItem(int id);
         public Task<CreateItemResponseDto?> addInventoryItem(CreateItemRequestDto itemRequest);
         public Task<Product?> deleteInventoryItem(int id);
         public Task<ItemDto?> updateInventoryItem(int id, CreateItemRequestDto item);
    }
}