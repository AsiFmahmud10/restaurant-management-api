using ProductManagement.Db;

namespace ProductManagement.Cart;

public class Cart : BaseEntity
{   
    
    public Guid? Token { get;set; }
    public bool? IsMerged {get; set; }
    
    public virtual ICollection<CartItem> CartItems { get; } = new List<CartItem>();

    public void AddCartItems(CartItem cartItem)
    {
        CartItems.Add(cartItem);
    }

    public void RemoveCartItems(CartItem cartItem)
    {
        new NotImplementedException();
    }

    public decimal GetTotalPrice()
    {
        return CartItems.Sum(item => item.GetTotalPrice());
    }
}
