using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Respository;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace asp_net_web_api.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        
        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper){
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<ItemDto> getInventoryItems(ProductQueryParameters queryParameters)
        {
            IQueryable<InventoryItem> inventoryItems = _unitOfWork.ItemsRepository.GetAll(item => item.Category).AsQueryable();
            inventoryItems = handleQuery(inventoryItems, queryParameters);
            List<ItemDto> items = _mapper.Map<List<ItemDto>>(inventoryItems.ToList());
            return items;
        }

        public ItemDto? getInventoryItem(int id)
        {
            var inventoryItem = _unitOfWork.ItemsRepository.GetById(id, item => item.Category);
            if(inventoryItem==null) return null;
            return _mapper.Map<ItemDto?>(inventoryItem);
        }

        public CreateItemResponseDto? addInventoryItem(CreateItemRequestDto itemRequest)
        {
            var category = _unitOfWork.CategoryRepository.GetById((int)itemRequest.CategoryId) 
                ?? throw new DbUpdateException("Category Not Found! Provide the correct category id");
            
            try
            {
                InventoryItem item = _mapper.Map<InventoryItem>(itemRequest);
                item.CreatedAt=item.ModifiedAt=DateTime.Now;
                _unitOfWork.ItemsRepository.Add(item);
                _unitOfWork.Complete();
            }
            catch(DbUpdateException){
                
                throw ;
            }

            var response = _mapper.Map(itemRequest, new CreateItemResponseDto(), opts => {
                opts.BeforeMap((src, dst)=>{
                    dst.Category = new CategoryDto(){Id = category.Id, Name=category.Name};
                    dst.CreatedAt = DateTime.Now;
                    dst.ModifiedAt = DateTime.Now;
                });
            });

            return response;
        }
        
        public  InventoryItem? deleteInventoryItem(int id)
        {
            InventoryItem? item = _unitOfWork.ItemsRepository.GetById(id);
            if(item == null) return null;
            _unitOfWork.ItemsRepository.Delete(item);
            _unitOfWork.Complete();
            return item;
        }

        public  ItemDto? updateInventoryItem(int id, CreateItemRequestDto itemRequest){
            if (id != itemRequest.Id) return null;
            
            var itemToUpdate = _unitOfWork.ItemsRepository.GetById(itemRequest.Id);
            if(itemToUpdate == null) return null;
                                                            
            var category = _unitOfWork.CategoryRepository.GetById((int)itemRequest.CategoryId)                                                                                                                                                                          
            ?? throw new DbUpdateException("Category Not Found! Provide the correct category id");
            
            _mapper.Map(itemRequest, itemToUpdate , opts=>{
                opts.BeforeMap((src, dst)=>{
                    dst.ModifiedAt = DateTime.Now;
                });
            });
            
            try{
                 _unitOfWork.ItemsRepository.Update(itemToUpdate);
                _unitOfWork.Complete();
            }
            catch (DbUpdateException){
                    throw;
            }
            
            var responseItemDto = _mapper.Map<ItemDto>(itemToUpdate);
            return responseItemDto;
        }

        private static IQueryable<InventoryItem> handleQuery(IQueryable<InventoryItem> inventoryItems, ProductQueryParameters queryParameters){

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
                if (typeof(InventoryItem).GetProperty(queryParameters.SortBy) != null)
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

            return inventoryItems.Include(i => i.Category);
        }
    }
}