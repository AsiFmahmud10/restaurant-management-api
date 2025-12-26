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

    public void ConfirmOrder(ConfirmOrderReq confirmOrderReq, ClaimsPrincipal principal)
    {
        Guid userId = AuthenticatedUserService.GetUserId(principal);
        var order = orderRepository.FindById(confirmOrderReq.OrderId) ??
                    throw new ResourceNotFoundException("Order not found");

        var payment = new Payment()
        {
            OrderId = order.Id,
            Order = order,
            Media = confirmOrderReq.Media,
            TransactionId = confirmOrderReq.TransactionId
        };

        order.Confirm(confirmOrderReq.Address, payment, confirmOrderReq.ReceiverNumber, confirmOrderReq.Note);
        orderRepository.Update(order);
    }

    public void UpdateStatusToPaidOrComplete(Guid orderId, OrderStatus status)
    {
        if ((status != OrderStatus.Paid) && (status != OrderStatus.Completed))
        {
            throw new ApplicationException("paid or completed order-status are allowed");
        }

        var order = orderRepository.FindById(orderId) ??
                    throw new ResourceNotFoundException("Order not found");
        ValidationForSpecificStatus(order, status);

        order.Status = status;
        orderRepository.Update(order);
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
}