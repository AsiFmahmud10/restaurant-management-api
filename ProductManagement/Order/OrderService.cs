using System.Security.Claims;
using ProductManagement.Cart;
using ProductManagement.Common.Model;
using ProductManagement.Exception;
using ProductManagement.Order.Dto;
using ProductManagement.Order.Enum;
using ProductManagement.Services.Common;
using ProductManagement.Services.Paging.Model;

namespace ProductManagement.Order;

using User;

public class OrderService(
    IOrderRepository orderRepository,
    IUserService userService,
    ICartService cartService
)
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

    public void ConfirmOrder(ConfirmOrderReq confirmOrderReq, ClaimsPrincipal principal)
    {
        Guid userId = AuthenticatedUserService.GetUserId(principal);
        var user = userService.FindUserWithCartDetails(userId) ?? throw new ResourceNotFoundException("User not found");

        var order = orderRepository.FindById(confirmOrderReq.OrderId) ??
                    throw new ResourceNotFoundException("Order not found");

        var payment = new Payment()
        {
            OrderId = order.Id,
            Order = order,
            Media = confirmOrderReq.Media,
            TransactionId = confirmOrderReq.TransactionId
        };

        order.Confirm(confirmOrderReq.Address, payment, confirmOrderReq.ReceiverNumber, confirmOrderReq.ReceiverName,
            confirmOrderReq.Note);
        if (user.Cart != null)
        {
            cartService.ClearCart(user.Cart.Id);
        }

        orderRepository.Update(order);
    }

    public void UpdateStatusToPaid(Guid orderId)
    {
        var order = orderRepository.GetOrderDetails(orderId) ??
                    throw new ResourceNotFoundException("Order not found");
        ValidationForSpecificStatus(order, OrderStatus.Paid);

        // update inventory product quantity
        order.OrderItems.ToList().ForEach(orderItem =>
        {
            var product = orderItem.Product;
            product.Quantity = product.Quantity - orderItem.Quantity;
        });

        order.Status = OrderStatus.Paid;
        order.StatusToPaidAt = DateTime.UtcNow;
        orderRepository.SaveChanges();
    }

    public void UpdateStatusToComplete(Guid orderId)
    {
        var order = orderRepository.FindById(orderId) ??
                    throw new ResourceNotFoundException("Order not found");

        ValidationForSpecificStatus(order, OrderStatus.Completed);

        order.Status = OrderStatus.Completed;
        order.StatusToCompletedAt = DateTime.UtcNow;
        orderRepository.SaveChanges();
    }


    public void UpdateStatusToShipped(Guid orderId, StatusToShippedRequest statusToShippedRequest)
    {
        var order = orderRepository.FindById(orderId) ??
                    throw new ResourceNotFoundException("Order not found");

        ValidationForSpecificStatus(order, OrderStatus.Shipped);
        if (!Uri.TryCreate(statusToShippedRequest.TrackingUrl, UriKind.Absolute, out Uri? uri) ||
            (!uri.Scheme.Equals(Uri.UriSchemeHttps)))
        {
            throw new ApplicationException("Invalid tracking url");
        }

        order.ShipmentTrackingUrl = statusToShippedRequest.TrackingUrl;
        order.Status = OrderStatus.Shipped;
        order.StatusToShippedAt = DateTime.UtcNow;
        orderRepository.Update(order);
    }

    private void ValidationForSpecificStatus(Order order, OrderStatus status)
    {
        if (status.Equals(OrderStatus.Paid) && !order.Status.Equals(OrderStatus.Confirmed))
        {
            throw new ApplicationException("Only confirmed orders status can be change to paid");
        }

        if (status.Equals(OrderStatus.Completed) && !order.Status.Equals(OrderStatus.Shipped))
        {
            throw new ApplicationException("Only shipped orders status can be changed to completed");
        }

        if (status.Equals(OrderStatus.Shipped) && !order.Status.Equals(OrderStatus.Paid))
        {
            throw new ApplicationException("Only Paid orders status can be changed to Shipped");
        }
    }

    public PaginationResult<GetOrderResponse> GetOrdersByStatus(OrderStatus orderStatus, PageData pageData)
    {
        if (orderStatus < OrderStatus.Confirmed)
        {
            throw new ApplicationException("This status is not allowed as filter");
        }

        return orderRepository.GetOrdersByStatus(orderStatus, pageData);
    }

    public PaginationResult<GetOrderResponse> GetAuthenticatedCustomerOrders(ClaimsPrincipal claimsPrincipal,
        PageData pageData)
    {
        var userId = AuthenticatedUserService.GetUserId(claimsPrincipal);
        var user = userService.FindById(userId, user => user.Orders) ??
                   throw new ResourceNotFoundException("User not found");

        return orderRepository.GetOrdersByUserId(userId, pageData);
    }
}