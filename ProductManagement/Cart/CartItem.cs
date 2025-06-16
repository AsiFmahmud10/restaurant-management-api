namespace ProductManagement.entity;

public class CartItem : BaseEntity
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }

    public decimal GetTotalPrice()
    {
        return this.Quantity * Product.Price;
    }
}