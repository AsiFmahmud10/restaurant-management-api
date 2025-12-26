using ProductManagement.Db;

namespace ProductManagement.Order;

public class OrderRepository(ApplicationDbContext dbContext) : GenericDbOperation<Order>(dbContext),IOrderRepository
{
   
}