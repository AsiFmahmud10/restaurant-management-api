using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Order.Dto;
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
    
    [Authorize(Roles = "customer")]
    [HttpPost("Confirm order")]
    [SwaggerOperation("Confirm order", description: "Confirm Order by providing payment & other information")]
    public IActionResult ConfirmOrder(ConfirmOrderReq confirmOrderReq)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        orderService.ConfirmOrder(confirmOrderReq, HttpContext.User);
        return Ok("Order confirmed successfully");
    }
}