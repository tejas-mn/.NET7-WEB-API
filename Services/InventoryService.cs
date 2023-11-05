using System.Runtime.CompilerServices;
using System;
using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Respository;
using Microsoft.EntityFrameworkCore;

namespace asp_net_web_api.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IUnitOfWork unitOfWork){
            _unitOfWork = unitOfWork;
        }

        public List<ItemDto> getInventoryItems(ProductQueryParameters queryParameters)
        {
            IQueryable<InventoryItem> inventoryItems = _unitOfWork.ItemsRepository.GetAll().AsQueryable();

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

            List<ItemDto> items = new();

            foreach (var item in inventoryItems)
            {
                var category = _unitOfWork.CategoryRepository.GetById(item.CategoryId);
                ItemDto it = new ItemDto(){
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    CategoryId = item.CategoryId,
                    Category = category!=null?new CategoryDto(){Id = category.Id, Name=category.Name} : new()
                };

                items.Add(it);
            }

            return  items;
        }

        public ItemDto? getInventoryItem(int id)
        {
            
            var inventoryItem = _unitOfWork.ItemsRepository.GetById(id);
            if(inventoryItem==null) return null;
            var category = _unitOfWork.CategoryRepository.GetById((int)inventoryItem.CategoryId);

            return new ItemDto(){
                Id = inventoryItem.Id,
                Name = inventoryItem.Name,
                Description = inventoryItem.Description,
                Price = inventoryItem.Price,
                CategoryId = inventoryItem.CategoryId,
                Category = category!=null?new CategoryDto(){Id = category.Id, Name=category.Name} : new()
            };

        }

        public CreateItemResponseDto? addInventoryItem(CreateItemRequestDto itemRequest)
        {
            var category = _unitOfWork.CategoryRepository.GetById((int)itemRequest.CategoryId) 
                ?? throw new DbUpdateException("Category Not Found! Provide the correct category id");
            
            try
            {
                InventoryItem item = new(){
                    Id = itemRequest.Id, 
                    Name = itemRequest.Name,
                    Description = itemRequest.Description,
                    Price = itemRequest.Price,
                    Sku = itemRequest.Sku,
                    CategoryId = itemRequest.CategoryId,
                    IsAvailable = itemRequest.IsAvailable,
                };
                _unitOfWork.ItemsRepository.Add(item);
                _unitOfWork.Complete();
            }
            catch(DbUpdateException ex){
                throw ex;
            }

            CreateItemResponseDto response = new(){
                Id = itemRequest.Id,
                Name = itemRequest.Name,
                Description = itemRequest.Description,
                Price = itemRequest.Price,
                CategoryId = itemRequest.CategoryId,
                Category = category!=null?new CategoryDto(){Id = category.Id, Name=category.Name} : new(),
                IsAvailable = itemRequest.IsAvailable,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            };

            return response;
        }
        
        
        public  InventoryItem? deleteInventoryItem(int id)
        {
            var item = _unitOfWork.ItemsRepository.GetById(id);

            if (item == null){
                return null;
            }
            
            _unitOfWork.ItemsRepository.Delete(item);

            _unitOfWork.Complete();

            return item;
        }

         public  InventoryItem? updateInventoryItem(int id, InventoryItem item){
            if (id != item.Id) return null;
            
            _unitOfWork.ItemsRepository.Update(item);
            
            var itemToUpdate = _unitOfWork.ItemsRepository.GetById(id);
        
            try{
                _unitOfWork.Complete();
            }
            catch (DbUpdateException){
                if (itemToUpdate == null){
                    return null;
                }
                else{
                    throw;
                }
            }

            var updatedItem = _unitOfWork.ItemsRepository.GetById(id);
            return updatedItem;
        }

    }
}