using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Category.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductManagement.Category;

[Route("api/v1/category")]
public class CategoryController(ICategoryService categoryService) : Controller
{
    [Authorize(Roles = "admin")]
    [HttpPost("create")]
    [SwaggerOperation("Create a category", description: "Create a category")]
    public IActionResult CreateCategory(CreateCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        categoryService.CreateCategory(request);
        return Ok("Category created successfully");
    }

    [AllowAnonymous()]
    [HttpGet("Get Categories")]
    [SwaggerOperation("Get Categories", description: "Get Categories")]
    public IActionResult GetCategory()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(categoryService.GetCategory());
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{categoryId}")]
    [SwaggerOperation("Delete a category", description: "Delete a category")]
    public IActionResult DeleteCategory(Guid categoryId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        categoryService.DeleteCategory(categoryId);
        return Ok("Category deleted successfully");
    }
}