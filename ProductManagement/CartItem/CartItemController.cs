using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.CartItem.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductManagement.CartItem;

[Route("api/v1/cart-items")]
public class CartItemController(ICartItemService cartItemService) : Controller
{
    [AllowAnonymous()]
    [HttpPut("{cartItemId}/quantity/{quantity}")]
    [SwaggerOperation("Increment/decrement quantity", description: "Increment/decrement quantity")]
    public IActionResult UpdateQuantity([FromRoute]Guid cartItemId,[FromBody] UpdateCartItemQuantityRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        cartItemService.UpdateQuantity(cartItemId, request.Quantity,HttpContext.User);
        return Ok("cart-item updated");
    }
}