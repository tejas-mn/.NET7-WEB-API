using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;

namespace asp_net_web_api.API.Services
{
    public interface ICategoryService
    {
        public Task<List<CategoryDto>> getCategories(ProductQueryParameters queryParameters);
        public CategoryDto? getCategory(int id);
        public Task<CategoryDto?> addCategory(CategoryDto itemRequest);
        public  Task<CategoryDto?> updateCategory(int id, CategoryDto itemRequest);
      
    }
}