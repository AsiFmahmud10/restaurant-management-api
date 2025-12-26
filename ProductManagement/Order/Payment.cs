using ProductManagement.Db;
using ProductManagement.Order.Enum;

namespace ProductManagement.Order;

public class Payment : BaseEntity
{
    public Guid OrderId{get;set;}
    public Order Order {get;set;}
    public PaymentMedia Media{get;set;}
    public string TransactionId { get; set; }

    public string GetPaymentTime()
    {
        return CreatedAt.ToString("dd.MM.YYY hh:mm:ss tt");
    }
}