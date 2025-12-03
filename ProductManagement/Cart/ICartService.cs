using System.Security.Claims;
using ProductManagement.Cart.Dto;

namespace ProductManagement.Cart;

public interface ICartService 
{
    AddToCartResponse AddProductToCart(AddProductToCartRequest request,ClaimsPrincipal user);
   
}