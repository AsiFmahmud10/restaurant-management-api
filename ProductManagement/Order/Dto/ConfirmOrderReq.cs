using ProductManagement.Order.Enum;

namespace ProductManagement.Order.Dto;

public class ConfirmOrderReq
{
    public Guid OrderId{get;set;}
    public String Address { get; set; }
    public String ReceiverNumber { get; set; }
    public String TransactionId { get; set; }
    public PaymentMedia Media { get; set; }
    public String Note { get; set; }
    
}