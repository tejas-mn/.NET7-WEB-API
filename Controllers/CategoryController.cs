using asp_net_web_api.API.DTO;
using asp_net_web_api.API.Models;
using asp_net_web_api.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using asp_net_web_api.API.Utility;
using System.Net;

namespace asp_net_web_api.API.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService inventoryService){
            _categoryService = inventoryService;
        }
        
        [Authorize(Roles = "Admin,User", Policy = "RequireReadPermission")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CategoryDto>))]
        public IActionResult GetCategories([FromQuery] ProductQueryParameters queryParameters)
        {
            var categories = _categoryService.getCategories(queryParameters);
            return Ok(categories);
        }

        [Authorize(Roles = "Admin,User", Policy = "RequireReadPermission")]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(object))]
        public IActionResult GetCategory(int id)
        {
            var category = _categoryService.getCategory(id);
            return Ok(category);
        }

        [Authorize(Roles = "Admin", Policy = "RequireWritePermission")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDto))]
        public  ActionResult<CategoryDto> AddCategory(CategoryDto categoryAddRequest)
        {
            var categorydto = _categoryService.addCategory(categoryAddRequest);
            return Ok(categorydto);
        }

        [Authorize(Roles = "Admin", Policy = "RequireReadPermission")]
        [Authorize(Roles = "Admin", Policy = "RequireWritePermission")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CategoryDto))]
        public ActionResult<CategoryDto> UpdateCategory(int id, CategoryDto categoryUpdateRequest)
        {
            var categorydto = _categoryService.updateCategory(id, categoryUpdateRequest);
            return Ok(categorydto);
        }
    }
}