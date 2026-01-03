namespace ProductManagement.Cart.Dto;

public class CartItemDetailsResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal ItemPrice { get; set; }
    public decimal TotalPrice { get; set; }
}