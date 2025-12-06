namespace ProductManagement.Cart.Dto;

public class CartDetailsResponse
{
    public Guid CartId {get; set;}
    public CartType Type { get; set; }
    public List<CartItemDetailsResponse> CartItemDetails { get; set; }
    public decimal TotalPrice {get; set;}
}