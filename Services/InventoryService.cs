using asp_net_web_api.API.Models;
using asp_net_web_api.API.Respository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace asp_net_web_api.API.Services
{
    public class InventoryService 
    {
        private UnitOfWork _unitOfWork;

        public InventoryService(UnitOfWork unitOfWork){
            _unitOfWork = unitOfWork;
        }

        public List<InventoryItem> getInventoryItems(ProductQueryParameters queryParameters)
        {
            IQueryable<InventoryItem> inventoryItems = (IQueryable<InventoryItem>)_unitOfWork.ItemsRepository.GetAll();

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

            inventoryItems = inventoryItems
                .Skip(queryParameters.Size * (queryParameters.Page - 1))
                .Take(queryParameters.Size);

            return  inventoryItems.Include(i => i.Category).ToList();
        }

        public InventoryItem? getInventoryItem(int id)
        {
             return  _unitOfWork.ItemsRepository.GetById(id);

            //  Include(i=>i.Category).FirstOrDefaultAsync(i=>i.Id==id);
        }

        public InventoryItem? addInventoryItem(InventoryItem item)
        {
            try{
                _unitOfWork.ItemsRepository.Add(item);
                _unitOfWork.Complete();
            }
            catch(DbUpdateException ex){
                throw ex;
            }

            return item;
        }
        
        
        public  InventoryItem? deleteInventoryItem(int id)
        {
            // var item = await _context.InventoryItems.FindAsync(id);

            var item = _unitOfWork.ItemsRepository.GetById(id);
            _unitOfWork.ItemsRepository.Delete(item);

            // if (item == null){
            //     return null;
            // }

            // _context.InventoryItems.Remove(item);

            _unitOfWork.Complete();

            return item;
        }

         public  InventoryItem? updateInventoryItem(int id, InventoryItem item){
            if (id != item.Id) return null;
            
            _unitOfWork.ItemsRepository.Update(item);
            
            // var itemToUpdate = _context.InventoryItems.Find(id);
            var itemToUpdate = _unitOfWork.ItemsRepository.GetById(id);
        
            try{
                _unitOfWork.Complete();
                // await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException){
                if (itemToUpdate == null){
                    return null;
                }
                else{
                    throw;
                }
            }

            // var updatedItem = _context.InventoryItems.Find(id);
            var updatedItem = _unitOfWork.ItemsRepository.GetById(id);
            return updatedItem;
        }

    }
}