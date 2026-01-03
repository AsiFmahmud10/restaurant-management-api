using ProductManagement.Order.Enum;

namespace ProductManagement.Order.Dto;

public class GetOrderResponse
{
    public string CustomerName { get; set; }
    public string RecieverName { get; set; }
    public string OrderNumber { get; set; }
    public string RecieverContactNumber { get; set; }
    public string Address { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public Guid OrderID { get; set; }

    public List<GetProductResponse> Products { get; set; } = new List<GetProductResponse>();
    public string? ShipmentTrackingUrl { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime StatusToConfirmedAt { get; set; }
    public DateTime? StatusToPaidAt { get; set; }
    public DateTime? StatusToShippedAt { get; set; }
    public DateTime? StatusToCompletedAt { get; set; }
}

public class GetProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPriceAtOrderTime { get; set; }
}