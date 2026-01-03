using System.Security.Claims;

namespace ProductManagement.CartItem;

public interface ICartItemService
{
    void UpdateQuantity(Guid cartItemId, int quantity, ClaimsPrincipal principal);
}