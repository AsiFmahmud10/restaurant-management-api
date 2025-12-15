using System.Security.Claims;
using ProductManagement.Cart.Dto;
using ProductManagement.Exception;
using ProductManagement.Services.Common;

namespace ProductManagement.Cart;

using User;
using Product;
using CartItem;

public class CartService(ICartRepository cartRepository, IUserService userService, IProductService productService)
    : ICartService
{
    public AddToCartResponse AddProductToCart(AddProductToCartRequest request, ClaimsPrincipal principal)
    {
        var product = productService.FindByProductId(request.ProductId) ??
                      throw new ResourceNotFoundException("Product not found");

        var newCartItem = new CartItem
        {
            Product = product,
            Quantity = request.Quantity,
        };

        if (AuthenticatedUserService.IsAuthenticated(principal))
        {
            var userId = AuthenticatedUserService.GetUserId(principal);
            var user = userService.FindUserWithCartDetails(userId) ??
                       throw new ResourceNotFoundException("User not found");

            user.Cart ??= new Cart() { Type = CartType.Customer };
            // if user cart has that product already increment previously added cart item
            var existedProduct =
                user.Cart.CartItems.SingleOrDefault(cartItem => cartItem.Product.Code.Equals(newCartItem.Product.Code));
            if (existedProduct is not null)
            {
                existedProduct.Quantity += request.Quantity;
            }
            else
            {
                user.Cart.AddCartItems(newCartItem);
            }

            userService.Update(user);

            return new AddToCartResponse()
            {
                CartId = user.Cart.Id,
                CartType = user.Cart.Type.ToString(),
            };
        }

        //if guest cart already present add to cart else create one 
        Guid? cartId = request.CartId;
        Cart guestCart;
        if (cartId.HasValue)
        {
            guestCart = cartRepository.GetCartDetails(cartId.Value) ??
                        throw new ResourceNotFoundException("Guest Cart not found");
            var existedProductInGuestCart =
                guestCart.CartItems.FirstOrDefault(cartItem => cartItem.Product.Code.Equals(newCartItem.Product.Code));
            // if product is already present in cart increment quantity
            // else add as a new product
            if (existedProductInGuestCart is not null)
            {
                existedProductInGuestCart.Quantity += request.Quantity;
            }
            else
            {
                guestCart.AddCartItems(newCartItem);
            }

            cartRepository.Update(guestCart);
        }
        else
        {
            // if guest cart not present create new guest cart & add new product
            guestCart = new Cart() { Type = CartType.Guest };
            guestCart.AddCartItems(newCartItem);
            cartRepository.save(guestCart);
        }

        return new AddToCartResponse()
        {
            CartId = guestCart.Id,
            CartType = guestCart.Type.ToString(),
        };
    }

    public CartDetailsResponse GetCartDetails(Guid? cartId, ClaimsPrincipal principal)
    {
        Cart cart;
        if (AuthenticatedUserService.IsAuthenticated(principal))
        {
            var userId = AuthenticatedUserService.GetUserId(principal);
            var user = userService.FindUserWithCartDetails(userId) ??
                       throw new ResourceNotFoundException("User not found");
            cart = user.Cart ?? throw new ResourceNotFoundException("User has no cart yet");
        }
        else
        {
            if (cartId is null)
            {
                throw new ResourceNotFoundException("Guest user does not have a cart yet");
            }

            cart = cartRepository.GetCartDetails(cartId.Value) ??
                   throw new ResourceNotFoundException("Cart not found with guest cart id " + cartId);
        }

        var cartItemDetailsResponse = cart.CartItems.ToList().Select(cartItem => new CartItemDetailsResponse()
        {
            Id = cartItem.Id,
            ProductId = cartItem.Product.Id,
            Quantity = cartItem.Quantity,
            ProductName = cartItem.Product.Name,
            ItemPrice = cartItem.Product.Price,
            TotalPrice = cartItem.GetTotalPrice()
        }).ToList();

        return new CartDetailsResponse()
        {
            CartId = cart.Id,
            CartItemDetails = cartItemDetailsResponse,
            Type = cart.Type,
            TotalPrice = cart.GetTotalPrice()
        };
    }

    public void ClearCart(Guid cartId)
    {
        Cart cart = cartRepository.FindById(cartId) ?? throw new ResourceNotFoundException("Cart not found");
        cartRepository.ClearCart(cart.Id);
    }

    private void MergeCart(AddProductToCartRequest request, User user, Product product)
    {
        Cart? existedCart = cartRepository.GetCartDetails(user.Cart!.Id);
        if (existedCart is null)
        {
            throw new ResourceNotFoundException("Cart not found");
        }

        Guid? cartId = request.CartId;

        if (cartId.HasValue)
        {
            Cart guestCart = cartRepository.GetCartDetails(cartId.Value) ??
                             throw new ResourceNotFoundException("Guest Cart not found");

            //merge guest cart with existed cart
            //jodi product id same hoy keep guest else keep both
            Dictionary<Guid, CartItem> productIdToCartItemFromExistedCart = existedCart.CartItems
                .ToList()
                .ToDictionary(cartItem => cartItem.Product.Id, carItem => carItem);

            guestCart.CartItems.ToList().ForEach(cartItem =>
                {
                    if (productIdToCartItemFromExistedCart.TryGetValue(cartItem.Id, out var value))
                    {
                        value.Quantity = cartItem.Quantity;
                    }
                    else
                    {
                        existedCart.AddCartItems(cartItem);
                    }
                }
            );

            cartRepository.Update(existedCart);
        }
    }
}