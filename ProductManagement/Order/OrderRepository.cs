using Microsoft.EntityFrameworkCore;
using ProductManagement.Db;

namespace ProductManagement.Order;

public class OrderRepository(ApplicationDbContext dbContext) : GenericDbOperation<Order>(dbContext),IOrderRepository
{
    public Order? GetOrderDetails(Guid orderId)
    {
        return dbContext.Orders
            .Include(order => order.OrderItems)
            .ThenInclude(orderItem => orderItem.Product)
            .SingleOrDefault(order => order.Id == orderId);
    }
}