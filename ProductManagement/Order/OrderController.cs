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


    [Authorize(Roles = "admin")]
    [HttpPut("{orderId}/update-status/paid")]
    [SwaggerOperation("Change order status", description: "Order status can change from confirmed to paid")]
    public IActionResult UpdateStatusToPaid(Guid orderId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        orderService.UpdateStatusToPaidOrComplete(orderId, OrderStatus.Paid);
        return Ok("Order status updated to paid successfully");
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{orderId}/update-status/shipped")]
    [SwaggerOperation("Change order status", description: "Order status can change from confirmed to paid")]
    public IActionResult UpdateStatusToShipped(Guid orderId, StatusToShippedRequest statusToShippedRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        orderService.UpdateStatusToShipped(orderId, statusToShippedRequest);
        return Ok("Order status updated to shipped successfully");
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{orderId}/update-status/complete")]
    [SwaggerOperation("Change order status", description: "Order status can change from shipped to complete")]
    public IActionResult UpdateStatusToComplete(Guid orderId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        orderService.UpdateStatusToPaidOrComplete(orderId, OrderStatus.Completed);
        return Ok("Order status updated to completed successfully");
    }
}