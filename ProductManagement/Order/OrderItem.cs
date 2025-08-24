namespace ProductManagement.Order;
using Product;
using Db;

public class OrderItem :  BaseEntity
{
    public Guid OrderId { get; set; }
    
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }

    public decimal GetTotalPrice()
    {
        return this.Quantity * Product.Price;
    }
}