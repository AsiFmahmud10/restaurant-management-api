namespace ProductManagement.Order;

using User;
using Db;


public class Order : BaseEntity
{
    public OrderStatus Status{get;set;}
    
    public Guid UserId { get; set; }
    public User User { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

    public decimal GetTotalPrice()
    {
        return OrderItems.Sum(item => item.GetTotalPrice());
    }
}