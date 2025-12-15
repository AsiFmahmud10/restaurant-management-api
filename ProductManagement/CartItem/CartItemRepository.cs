using ProductManagement.Db;

namespace ProductManagement.CartItem;

public class CartItemRepository(ApplicationDbContext dbContext)
    : GenericDbOperation<CartItem>(dbContext), ICartItemRepository
{
}