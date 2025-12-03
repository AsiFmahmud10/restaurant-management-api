using Microsoft.EntityFrameworkCore;
using ProductManagement.Db;

namespace ProductManagement.Cart;

public class CartRepository(ApplicationDbContext dbContext) : GenericDbOperation<Cart>(dbContext),ICartRepository
{
    public bool CategoryHasProducts(Guid categoryId)
    {
        return dbContext.Products.Any(p => p.CategoryId.Equals(categoryId));
    }

    public Cart? GetCartDetails(Guid cartId)
    {
        return dbContext.Carts
            .Include(cart => cart.CartItems)
            .ThenInclude(cartItem => cartItem.Product)
            .SingleOrDefault(c => c.Id == cartId);
    }
}