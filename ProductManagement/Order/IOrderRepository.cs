using ProductManagement.Db;

namespace ProductManagement.Order;

public interface IOrderRepository : IGenericDbOperation<Order>
{
    public Order? GetOrderDetails(Guid orderId);
}