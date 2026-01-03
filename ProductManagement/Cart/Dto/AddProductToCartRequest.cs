namespace ProductManagement.Cart.Dto;

public class AddProductToCartRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public Guid? CartId { get; set; }
}

 