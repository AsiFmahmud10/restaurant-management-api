using System.Security.Claims;
using ProductManagement.Cart.Dto;

namespace ProductManagement.Cart;

public interface ICartService 
{
    AddToCartResponse AddProductToCart(AddProductToCartRequest request,ClaimsPrincipal user);
    CartDetailsResponse GetCartDetails(Guid? cartId, ClaimsPrincipal principal);
    void ClearCart(Guid cartId);
}