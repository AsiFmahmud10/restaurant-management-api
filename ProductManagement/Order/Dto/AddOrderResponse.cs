using ProductManagement.Order.Enum;

namespace ProductManagement.Order.Dto;

public class AddOrderResponse
{
    public Guid OrderId { get; set; }
    public string OrderIdentifer { get; set; }
    public bool IsPriceUpdated { get;set; }
    public OrderStatus OrderStatus { get; set; }
}