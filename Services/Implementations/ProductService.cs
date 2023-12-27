using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Respository;
using asp_net_web_api.API.ErrorHandling;
using Microsoft.EntityFrameworkCore;
using asp_net_web_api.API.Utility;
using AutoMapper;

namespace asp_net_web_api.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper){
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ItemDto>> getInventoryItems(ProductQueryParameters queryParameters)
        {
            IEnumerable<Product> inventoryItems = await _unitOfWork.ItemsRepository.GetAll(item => item.Category);
            inventoryItems = handleQuery(inventoryItems.AsQueryable(), queryParameters);
            List<ItemDto> items = _mapper.Map<List<ItemDto>>(inventoryItems.ToList());
            return items;
        }

        public ItemDto? getInventoryItem(int id)
        {
            var inventoryItem = _unitOfWork.ItemsRepository.GetById(id, item => item.Category);
            if(inventoryItem==null) throw new ItemNotFoundException($"Requested Item {id} Not Found");
            return _mapper.Map<ItemDto?>(inventoryItem);
        }

        public async Task<CreateItemResponseDto?> addInventoryItem(CreateItemRequestDto itemRequest)
        {
            var validator = new CreateItemRequestValidator(_unitOfWork);
            var validationResult = validator.ValidateAndThrow(itemRequest);
            
            if (!validationResult.IsValid) throw new Exception("One or More Valdations failed");
            
            Product item = _mapper.Map<Product>(itemRequest);
            item.CreatedAt=item.ModifiedAt=DateTime.Now;

            _unitOfWork.ItemsRepository.Add(item);
            await _unitOfWork.SaveAsync();
 
            var addedItem = _unitOfWork.ItemsRepository.GetById(itemRequest.Id);
            var response = _mapper.Map<CreateItemResponseDto>(addedItem);

            return response;
        }
        
        public  async Task<Product?> deleteInventoryItem(int id)
        {
            Product? item = _unitOfWork.ItemsRepository.GetById(id);
            if(item == null) throw new ItemNotFoundException($"Requested Item {id} Not Found");
            _unitOfWork.ItemsRepository.Delete(item);
            await _unitOfWork.SaveAsync();
            return item;
        }

        public async Task<ItemDto?> updateInventoryItem(int id, CreateItemRequestDto itemRequest)
        {
            if (id != itemRequest.Id) return null;
            
            var itemToUpdate = _unitOfWork.ItemsRepository.GetById(itemRequest.Id, item=>item.Category);
            if(itemToUpdate == null) throw new ItemNotFoundException($"Requested Item {id} Not Found");
            
            var categoryNotFound = _unitOfWork.CategoryRepository.GetById(itemRequest.CategoryId)==null;
            if(categoryNotFound) throw new CategoryNotFoundException($"Category {itemRequest.CategoryId} Not found");         

            _mapper.Map(itemRequest, itemToUpdate , opts=>{
                opts.BeforeMap((src, dst)=>{
                    dst.ModifiedAt = DateTime.Now;
                });
            });

            _unitOfWork.ItemsRepository.Update(itemToUpdate);
            await _unitOfWork.SaveAsync();

            var updatedItem = getInventoryItem(itemRequest.Id);
            var responseItemDto = _mapper.Map<ItemDto>(updatedItem);

            return responseItemDto;
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