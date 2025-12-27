using System.Security.Claims;
using ProductManagement.Order.Dto;

namespace ProductManagement.Order;

public interface IOrderService 
{
    AddOrderResponse AddOrder(ClaimsPrincipal httpContextUser);
    void ConfirmOrder(ConfirmOrderReq confirmOrderReq, ClaimsPrincipal httpContextUser);
    void UpdateStatusToPaid(Guid orderId);
    void UpdateStatusToComplete(Guid orderId);
    void UpdateStatusToShipped(Guid orderId,StatusToShippedRequest statusToShippedRequest);
}