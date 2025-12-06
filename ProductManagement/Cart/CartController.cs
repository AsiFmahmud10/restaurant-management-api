using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Cart.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductManagement.Cart;

[Route("api/v1/cart")]
public class CatController(ICartService cartService) : Controller
{
    [AllowAnonymous()]
    [HttpPost("products/add")]
    [SwaggerOperation("Add product to cart",description:"Add product to cart")]
    public IActionResult AddProductToCart([FromBody]AddProductToCartRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return Ok(cartService.AddProductToCart(request,HttpContext.User));
    }
    
    [AllowAnonymous()]
    [HttpPost()]
    [SwaggerOperation("Get Cart Details",description:"Get cart Details")]
    public IActionResult GetCartDetails([FromQuery] Guid? cartId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return Ok(cartService.GetCartDetails(cartId,HttpContext.User));
    }
}