using System.Security.Claims;
using ProductManagement.Common.Model;
using ProductManagement.Order.Dto;
using ProductManagement.Order.Enum;
using ProductManagement.Services.Paging.Model;

namespace ProductManagement.Order;

public interface IOrderService 
{
    AddOrderResponse AddOrder(ClaimsPrincipal httpContextUser);
    void ConfirmOrder(ConfirmOrderReq confirmOrderReq, ClaimsPrincipal httpContextUser);
    void UpdateStatusToPaid(Guid orderId);
    void UpdateStatusToComplete(Guid orderId);
    void UpdateStatusToShipped(Guid orderId,StatusToShippedRequest statusToShippedRequest);
    PaginationResult<GetOrderResponse> GetOrdersByStatus(OrderStatus orderStatus,PageData pageData);
    PaginationResult<GetOrderResponse> GetAuthenticatedCustomerOrders(ClaimsPrincipal httpContextUser, PageData pageData);
}