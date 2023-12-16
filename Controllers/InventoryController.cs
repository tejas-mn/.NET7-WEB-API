using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using asp_net_web_api.API.Utility;

namespace asp_net_web_api.API.Controllers
{
    [Authorize]
    public class InventoryController : BaseController
    {
        private readonly IInventoryService _inventoryService;
        public InventoryController(IInventoryService inventoryService){
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// Get list of all the items present in Inventory
        /// </summary>
        /// <remarks>Get list of all the items present in Inventory</remarks>
        /// <returns>Returns list of items.</returns>
        /// <response code="200">Returns list of items.</response>
        /// <response code="400">If there are no items.</response>  
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ItemDto>))]
        public IActionResult GetInventoryItems([FromQuery] ProductQueryParameters queryParameters)
        {
            var inventoryItems = _inventoryService.getInventoryItems(queryParameters);
            return Ok(inventoryItems);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(object))]
        public IActionResult GetInventoryItem(int id)
        {
            if (id==0) return BadRequest("Id 0 not allowed.");
            var item =  _inventoryService.getInventoryItem(id);
            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateItemResponseDto))]
        public  ActionResult<CreateItemResponseDto> AddInventoryItem(CreateItemRequestDto itemRequest)
        {
            if (itemRequest.Id==0) return BadRequest("Id 0 not allowed.");
            var newItem = _inventoryService.addInventoryItem(itemRequest);
            return Ok(newItem);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(object))]
        public IActionResult DeleteInventoryItem(int id)
        {
            if(id==0) return BadRequest("Id 0 not allowed.");
            var item = _inventoryService.deleteInventoryItem(id);
            return Ok($"Item {id} deleted");
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ItemDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ItemDto))]
        public ActionResult<ItemDto> UpdateInventoryItem(int id, CreateItemRequestDto itemRequest){
            if (id != itemRequest.Id || itemRequest.Id==0) return BadRequest($"Wrong item id {itemRequest.Id} in request and url");
            var updatedItemDto =  _inventoryService.updateInventoryItem(id, itemRequest); 
            return Ok(updatedItemDto);
        }
    }
}