using System.Security.Claims;
using ProductManagement.Cart;
using ProductManagement.Exception;
using ProductManagement.Order.Dto;
using ProductManagement.Services.Common;

namespace ProductManagement.Order;

using User;

public class OrderService(IOrderRepository orderRepository, IUserService userService, ICartService cartService)
    : IOrderService
{
    public AddOrderResponse AddOrder(ClaimsPrincipal principal)
    {
        Guid userId = AuthenticatedUserService.GetUserId(principal);
        var user = userService.FindUserWithCartDetails(userId) ?? throw new ResourceNotFoundException("User not found");

        if (user.Cart == null || user.Cart.CartItems.Count == 0)
        {
            throw new ApplicationException("Please add Product first");
        }

        Order newOrder = new Order() { Status = OrderStatus.Pending };
        newOrder.AddIdentifier();
        
        bool isPriceUpdated = false;
       
        // update item min quantity available in store
        user.Cart.CartItems.ToList().ForEach(item =>
            {
                var availableQuantity = item.Product.Quantity;
                var reqQuantity = item.Quantity;

                if (availableQuantity < reqQuantity)
                {
                    reqQuantity = availableQuantity;
                    isPriceUpdated = true;
                }

                OrderItem newOrderItem = new OrderItem()
                {
                    ProductId = item.ProductId,
                    Quantity = reqQuantity,
                    PriceAtPurchaseTime = item.Product.Price,
                };
                newOrder.OrderItems.Add(newOrderItem);
            }
        );
        user.AddOrder(newOrder);
        userService.Update(user);

        return new AddOrderResponse()
        {
            OrderId = newOrder.Id,
            IsPriceUpdated = isPriceUpdated,
            OrderIdentifer = newOrder.Identifier,
            OrderStatus = OrderStatus.Pending
        };
        
    }
}