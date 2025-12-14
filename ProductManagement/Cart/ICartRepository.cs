using ProductManagement.Db;

namespace ProductManagement.Cart;

public interface ICartRepository : IGenericDbOperation<Cart>
{
    public Cart? GetCartDetails(Guid cartId);
    void ClearCart(Guid cartId);
}