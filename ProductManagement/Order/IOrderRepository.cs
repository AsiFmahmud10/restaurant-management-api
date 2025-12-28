using ProductManagement.Common.Model;
using ProductManagement.Db;
using ProductManagement.Order.Dto;
using ProductManagement.Order.Enum;
using ProductManagement.Services.Paging.Model;

namespace ProductManagement.Order;

public interface IOrderRepository : IGenericDbOperation<Order>
{
    public Order? GetOrderDetails(Guid orderId);
    public PaginationResult<AdminGetOrdersResponse> GetOrdersByStatus(OrderStatus orderStatus, PageData pageData);
}