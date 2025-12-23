namespace ProductManagement.Order;

using User;
using Db;


public class Order : BaseEntity
{
    public OrderStatus Status{get;set;}
    
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Identifier { get; set; }
    public string? Address { get; set; }
    public void AddIdentifier()
    {
        this.Identifier = Guid.CreateVersion7().ToString().Substring(0,9);
    }
    public virtual ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

    public decimal GetTotalPrice()
    {
        return OrderItems.Sum(item => item.GetTotalPrice());
    }

    public void ConfirmOrder()
    {
        if (Address is null || String.IsNullOrWhiteSpace(Address))
        {
            throw new ApplicationException("Address is null or empty");
        }
        this.Status = OrderStatus.Confirmed;
    }
}