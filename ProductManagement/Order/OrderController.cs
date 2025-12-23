using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductManagement.Order;

[Route("api/v1/orders")]
public class OrderController(IOrderService orderService) : Controller
{
    [Authorize(Roles = "customer")]
    [HttpPost("add")]
    [SwaggerOperation("Add Order", description: "Add Order")]
    public IActionResult AddOrder()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        return Ok(orderService.AddOrder(HttpContext.User));
    }

}