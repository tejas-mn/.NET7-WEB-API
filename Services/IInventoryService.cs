using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace asp_net_web_api.API.Services
{
    public interface IInventoryService
    {
         public List<ItemDto> getInventoryItems(ProductQueryParameters queryParameters);
         public ItemDto? getInventoryItem(int id);
         public InventoryItem? addInventoryItem(InventoryItem item);
         public InventoryItem? deleteInventoryItem(int id);
         public InventoryItem? updateInventoryItem(int id, InventoryItem item);
    }
}