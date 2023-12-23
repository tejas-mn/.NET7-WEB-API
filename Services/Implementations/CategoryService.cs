using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Respository;
using asp_net_web_api.API.ErrorHandling;
using Microsoft.EntityFrameworkCore;
using asp_net_web_api.API.Utility;
using AutoMapper;

namespace asp_net_web_api.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper){
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<CategoryDto> getCategories(ProductQueryParameters queryParameters)
        {
            List<Category> inventoryItems = (List<Category>)_unitOfWork.CategoryRepository.GetAll();
            List<CategoryDto> items = _mapper.Map<List<CategoryDto>>(inventoryItems.ToList());
            return items;
        }

        public CategoryDto? getCategory(int id)
        {
            var category = _unitOfWork.CategoryRepository.GetById(id);
            if(category==null) throw new CategoryNotFoundException($"category {id} not found!");
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }

        public CategoryDto? addCategory(CategoryDto categoryRequest)
        {
            var category = _mapper.Map<Category>(categoryRequest);
            _unitOfWork.CategoryRepository.Add(category);
            _unitOfWork.Complete();
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }

        public  CategoryDto? updateCategory(int id, CategoryDto categoryRequest)
        {
            var category = _unitOfWork.CategoryRepository.GetById(id);
            if(category==null) throw new CategoryNotFoundException($"category {id} not found!");
            category = _mapper.Map<Category>(categoryRequest);
            _unitOfWork.CategoryRepository.Update(category);
            _unitOfWork.Complete();
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }

        private static IQueryable<Product> handleQuery(IQueryable<Product> inventoryItems, ProductQueryParameters queryParameters)
        {
            if(queryParameters.MinPrice != null){
                inventoryItems = inventoryItems.Where(p => p.Price >= queryParameters.MinPrice);
            }

            if(queryParameters.MaxPrice != null){
                inventoryItems = inventoryItems.Where(p => p.Price <= queryParameters.MaxPrice);
            }

            if(!string.IsNullOrEmpty(queryParameters.Name)){
                inventoryItems = inventoryItems.Where(p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
            }

            if(!string.IsNullOrEmpty(queryParameters.Sku)){
                inventoryItems = inventoryItems.Where(p => p.Sku.ToLower().Contains(queryParameters.Sku.ToLower()));
            }

            if(!string.IsNullOrEmpty(queryParameters.SearchTerm)){
                inventoryItems = inventoryItems.Where(p => p.Name.ToLower().Contains(queryParameters.SearchTerm.ToLower()) || p.Sku.ToLower().Contains(queryParameters.SearchTerm.ToLower()));
            }
            
            if(!string.IsNullOrEmpty(queryParameters.Category)){
                inventoryItems = inventoryItems.Where(p => p.Category!=null && p.Category.Name.ToLower().Contains(queryParameters.Category.ToLower()));
            }

            if(!string.IsNullOrEmpty(queryParameters.SortBy)){
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    inventoryItems = inventoryItems.OrderByCustom(
                        queryParameters.SortBy,
                        queryParameters.SortOrder
                    );
                }
            }

            inventoryItems = inventoryItems
                .Skip(queryParameters.Size * (queryParameters.Page - 1))
                .Take(queryParameters.Size);

            return inventoryItems;
        }
    }
}