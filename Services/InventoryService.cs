using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Respository;
using asp_net_web_api.API.ErrorHandling;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using AutoMapper;

namespace asp_net_web_api.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ILogger<InventoryService> Logger { get; }

        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<InventoryService> logger){
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            Logger = logger;
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
            if(inventoryItem==null) throw new ItemNotFoundException("Requested Item Not Found");
            return _mapper.Map<ItemDto?>(inventoryItem);
        }

        public CreateItemResponseDto? addInventoryItem(CreateItemRequestDto itemRequest)
        {
            var itemExists = getInventoryItem(itemRequest.Id);
            if(itemExists!=null) throw new Exception($"Item {itemRequest.Id} already exists");
            
            var categoryExists = _unitOfWork.CategoryRepository.GetById(itemRequest.CategoryId);
            if(categoryExists==null) throw new CategoryNotFoundException($"Category {itemRequest.CategoryId} Not found");

            InventoryItem item = _mapper.Map<InventoryItem>(itemRequest);
            item.CreatedAt=item.ModifiedAt=DateTime.Now;
            
            try
            {
                _unitOfWork.ItemsRepository.Add(item);
                _unitOfWork.Complete();
            }
            catch(DbUpdateException ex){
                 if (ex.InnerException is SqliteException){
                    throw new CategoryNotFoundException($"Category {itemRequest.CategoryId} Not found");
                 }
                 throw ex;
            }

            var addedItem = getInventoryItem(itemRequest.Id);
            var response = _mapper.Map<CreateItemResponseDto>(addedItem);

            return response;
        }
        
        public  InventoryItem? deleteInventoryItem(int id)
        {
            InventoryItem? item = _unitOfWork.ItemsRepository.GetById(id);
            if(item == null) throw new ItemNotFoundException($"Requested Item {id} Not Found");
            _unitOfWork.ItemsRepository.Delete(item);
            _unitOfWork.Complete();
            return item;
        }

        public  ItemDto? updateInventoryItem(int id, CreateItemRequestDto itemRequest)
        {
            if (id != itemRequest.Id) return null;
            
            var itemToUpdate = _unitOfWork.ItemsRepository.GetById(itemRequest.Id, item=>item.Category);
            if(itemToUpdate == null) throw new ItemNotFoundException("Requested Item Not Found");
                                                            
            _mapper.Map(itemRequest, itemToUpdate , opts=>{
                opts.BeforeMap((src, dst)=>{
                    dst.ModifiedAt = DateTime.Now;
                });
            });
            
            try{
                 _unitOfWork.ItemsRepository.Update(itemToUpdate);
                _unitOfWork.Complete();
            }
            catch(DbUpdateException ex){
                if (ex.InnerException is SqliteException){
                   throw new CategoryNotFoundException("Category Not found");
                }
                throw ex;
            }
            
            var updatedItem = getInventoryItem(itemRequest.Id);
            var responseItemDto = _mapper.Map<ItemDto>(updatedItem);

            return responseItemDto;
        }

        private static IQueryable<InventoryItem> handleQuery(IQueryable<InventoryItem> inventoryItems, ProductQueryParameters queryParameters)
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