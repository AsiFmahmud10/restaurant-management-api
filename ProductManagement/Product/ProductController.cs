using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Common.Model;
using ProductManagement.Product.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductManagement.Product;

[Route("api/v1/products")]
public class ProductController(IProductService productService) : Controller
{
    [Authorize(Roles = "admin")]
    [HttpPost("add")]
    [SwaggerOperation("Add a Product", description: "Add a Product")]
    public IActionResult AddProduct(CreateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        productService.AddProduct(request);
        return Ok("Product added successfully");
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{productId}")]
    [SwaggerOperation("Update Products", description: "Update product")]
    public IActionResult UpdateProduct(Guid productId, [FromBody] UpdateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        productService.UpdateProduct(productId, request);
        return Ok("Category deleted successfully");
    }

    [AllowAnonymous]
    [HttpGet("category/{categoryId}")]
    [SwaggerOperation("Get Products by category", description: "Get Products by category")]
    public IActionResult GetProduct(Guid categoryId,[FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(productService.GetProductsByCategory(categoryId,new PageData(pageNumber, pageSize)));
    }
}