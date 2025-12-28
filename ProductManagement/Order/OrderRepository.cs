using Microsoft.EntityFrameworkCore;
using ProductManagement.Common.Model;
using ProductManagement.Db;
using ProductManagement.Order.Dto;
using ProductManagement.Order.Enum;
using ProductManagement.Services.Paging.Model;

namespace ProductManagement.Order;

public class OrderRepository(ApplicationDbContext dbContext) : GenericDbOperation<Order>(dbContext), IOrderRepository
{
    public Order? GetOrderDetails(Guid orderId)
    {
        return dbContext.Orders
            .Include(order => order.OrderItems)
            .ThenInclude(orderItem => orderItem.Product)
            .SingleOrDefault(order => order.Id == orderId);
    }

    public PaginationResult<AdminGetOrdersResponse> GetOrdersByStatus(OrderStatus orderStatus, PageData pageData)
    {
        var pageNumber = pageData.PageNumber;
        var pageSize = pageData.PageSize;

        var query = dbContext.Orders
            .Include(order => order.OrderItems)
            .ThenInclude(orderItem => orderItem.Product)
            .Include(order => order.User)
            .Where(order => order.Status == orderStatus)
            .OrderByDescending(order => order.ModifiedAt).ThenByDescending(order => order.CreatedAt);

        var totalRow = query.Count();
        var totalPages = (int)Math.Ceiling(totalRow / (double)pageSize);

        var result = query.Select(order => ConvertToAdminOrderResponse(order))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToList();

        return new PaginationResult<AdminGetOrdersResponse>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPage = totalPages,
            Result = result,
            DataCount = result.Count()
        };
    }

    private static AdminGetOrdersResponse ConvertToAdminOrderResponse(Order order)
    {
        List<AdminProductResponse> adminProductResponse = order.OrderItems.Select(item => new AdminProductResponse()
        {
            Id = item.Product.Id,
            Name = item.Product.Name,
            Quantity = item.Quantity,
            UnitPriceAtOrderTime = item.PriceAtPurchaseTime
        }).ToList();

        AdminGetOrdersResponse response = new AdminGetOrdersResponse
        {
            OrderID = order.Id,
            OrderNumber = order.Identifier,
            CustomerName = order.User.GetFullName(),
            RecieverName = order.ReceieverName,
            RecieverContactNumber = order.ReceiverNumber,
            TotalPrice = order.GetTotalPrice(),
            Address = order.Address,
            OrderStatus = order.Status,
            ShipmentTrackingUrl = order.ShipmentTrackingUrl,
            Products = adminProductResponse
        };

        return response;
    }
}