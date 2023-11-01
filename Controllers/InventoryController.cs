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
        public ActionResult<IEnumerable<InventoryItem>> GetInventoryItems([FromQuery] ProductQueryParameters queryParameters)
        {
            var inventoryItems = _inventoryService.getInventoryItems(queryParameters);
            _logger.LogInformation("GetInventoryItems invoked");
            return Ok(inventoryItems);
        }

        [HttpGet("{id}")]
        public IActionResult GetInventoryItem(int id)
        {
            var item =  _inventoryService.getInventoryItem(id);
            if (item == null) return NotFound("The requested item not found");
            _logger.LogInformation("GetInventoryItem invoked");
            return Ok(item);
        }

        [HttpPost]
        public  ActionResult<InventoryItem> AddInventoryItem(InventoryItem item)
        {
            var newItem = _inventoryService.addInventoryItem(item);
            _logger.LogInformation("CreateInventoryItem invoked");
            return CreatedAtAction("GetInventoryItem",new { id = item.Id }, newItem);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteInventoryItem(int id)
        {
            var item = _inventoryService.deleteInventoryItem(id);
            if (item == null) return NotFound();
            _logger.LogInformation("DeleteInventoryItem invoked");
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult<InventoryItem> UpdateInventoryItem(int id, InventoryItem item){
            if (id != item.Id) return BadRequest();
            var updatedItem =  _inventoryService.updateInventoryItem(id, item); 
            if(updatedItem==null) return NotFound();
            _logger.LogInformation("UpdateInventoryItem invoked");
            return Ok(updatedItem);
        }
    }
}