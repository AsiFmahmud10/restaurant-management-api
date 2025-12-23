using System.Security.Claims;
using ProductManagement.Order.Dto;

namespace ProductManagement.Order;

public interface IOrderService 
{
    AddOrderResponse AddOrder(ClaimsPrincipal httpContextUser);
}