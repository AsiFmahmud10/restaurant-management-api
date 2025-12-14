using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Cart.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductManagement.Cart;

[Route("api/v1/cart")]
public class CartController(ICartService cartService) : Controller
{
    [AllowAnonymous()]
    [HttpPost("products/add")]
    [SwaggerOperation("Add product to cart", description: "Add product to cart")]
    public IActionResult AddProductToCart([FromBody] AddProductToCartRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(cartService.AddProductToCart(request, HttpContext.User));
    }

    [AllowAnonymous()]
    [HttpPost()]
    [SwaggerOperation("Get Cart Details", description: "Get cart Details")]
    public IActionResult GetCartDetails([FromQuery] Guid? cartId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(cartService.GetCartDetails(cartId, HttpContext.User));
    }

    [AllowAnonymous()]
    [HttpDelete()]
    [SwaggerOperation("Clear cart",
        description: "Cart Id is required for guest user. For authenticated user dont need to pass the cart id")]
    public IActionResult ClearCart(Guid cartId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        cartService.ClearCart(cartId);
        return Ok("Cart is clear successfully");
    }
}