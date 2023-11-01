using asp_net_web_api.API.Models;
using asp_net_web_api.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace asp_net_web_api.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;
        private readonly AppDbContext _context;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(InventoryService inventoryService, AppDbContext context, ILogger<InventoryController> logger){
            _inventoryService = inventoryService;
            _context = context;
            _logger = logger;
            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public ActionResult<IEnumerable<InventoryItem>> GetInventoryItems([FromQuery] ProductQueryParameters queryParameters)
        {
            var inventoryItems = _inventoryService.getInventoryItems(queryParameters);
            _logger.LogInformation("GetInventoryItems invoked");
            return Ok(inventoryItems);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryItem(int id)
        {
            var item = await _inventoryService.getInventoryItem(id);

            if (item == null){
                return NotFound("The requested item not found");
            }
            
            _logger.LogInformation("GetInventoryItem invoked");
            return Ok(item);
        }

        [HttpPost]
        public  async Task<ActionResult<InventoryItem>> AddInventoryItem(InventoryItem item)
        {
           
            var newItem = await _inventoryService.addInventoryItem(item);
            _logger.LogInformation("CreateInventoryItem invoked");
            return CreatedAtAction("GetInventoryItem",new { id = item.Id }, newItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem(int id)
        {
            var item = _inventoryService.deleteInventoryItem(id);

            if (item == null){
                return NotFound();
            }

            _logger.LogInformation("DeleteInventoryItem invoked");

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryItem>> UpdateInventoryItem(int id, InventoryItem item){
            if (id != item.Id) return BadRequest();
            var updatedItem = await _inventoryService.updateInventoryItem(id, item); 
            if(updatedItem==null) return NotFound();

            _logger.LogInformation("UpdateInventoryItem invoked");
            
            return Ok(updatedItem);
        }
    }
}