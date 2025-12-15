using System.Security.Claims;
using ProductManagement.Exception;
using ProductManagement.Services.Common;
using ProductManagement.User;

namespace ProductManagement.CartItem;

public class CartItemService(IUserService userService, ICartItemRepository cartItemRepository) : ICartItemService
{
    public void UpdateQuantity(Guid cartItemId, int quantity, ClaimsPrincipal principal)
    {
        var cartItem = cartItemRepository.FindById(cartItemId, cartItems => cartItems.Product) ??
                       throw new ResourceNotFoundException("cartItem not found");

        if (AuthenticatedUserService.IsAuthenticated(principal))
        {
            var userId = AuthenticatedUserService.GetUserId(principal);
            var user = userService.FindUserWithCartDetails(userId) ??
                       throw new ResourceNotFoundException("User not found");
            var cart = user.Cart ?? throw new ResourceNotFoundException("User has no cart yet");

            if (!cart.CartItems.Contains(cartItem))
            {
                throw new ResourceNotFoundException("User cart dont have that cart item");
            }
        }

        var availableStock = cartItem.Product.Quantity;
        if (quantity.Equals(0))
        {
            cartItemRepository.Delete(cartItem);
            return;
        }

        if (quantity >= availableStock)
        {
            throw new ApplicationException($"Only {availableStock} item(s) available in stock");
        }

        cartItem.Quantity = quantity;
        cartItemRepository.Update(cartItem);
    }
}