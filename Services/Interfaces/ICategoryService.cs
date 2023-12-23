using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;

namespace asp_net_web_api.API.Services
{
    public interface ICategoryService
    {
        public List<CategoryDto> getCategories(ProductQueryParameters queryParameters);
        public CategoryDto? getCategory(int id);
        public CategoryDto? addCategory(CategoryDto itemRequest);
        public  CategoryDto? updateCategory(int id, CategoryDto itemRequest);
      
    }
}