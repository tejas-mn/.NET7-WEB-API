using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_web_api.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger){
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ItemDto>))]
        public IActionResult GetInventoryItems([FromQuery] ProductQueryParameters queryParameters)
        {
            var inventoryItems = _inventoryService.getInventoryItems(queryParameters);
            _logger.LogInformation("GetInventoryItems invoked");
            return Ok(inventoryItems);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(object))]
        public IActionResult GetInventoryItem(int id)
        {
            var item =  _inventoryService.getInventoryItem(id);
            if (item == null) return NotFound("The requested item not found");
            _logger.LogInformation("GetInventoryItem invoked");
            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateItemResponseDto))]
        public  ActionResult<CreateItemResponseDto> AddInventoryItem(CreateItemRequestDto item)
        {
            var newItem = _inventoryService.addInventoryItem(item);
            _logger.LogInformation("CreateInventoryItem invoked");
            return Ok(newItem);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(object))]
        public IActionResult DeleteInventoryItem(int id)
        {
            var item = _inventoryService.deleteInventoryItem(id);
            if (item == null) return NotFound("The requested item to delete was not found");
            _logger.LogInformation("DeleteInventoryItem invoked");
            return Ok("Item deleted");
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public ActionResult<ItemDto> UpdateInventoryItem(int id, CreateItemRequestDto itemRequest){
            if (id != itemRequest.Id) return BadRequest("Wrong item id on request and url");
            var updatedItemDto =  _inventoryService.updateInventoryItem(id, itemRequest); 
            if(updatedItemDto==null) return NotFound("The requested item to update was not found");
            _logger.LogInformation("UpdateInventoryItem invoked");
            return Ok(updatedItemDto);
        }
    }
}