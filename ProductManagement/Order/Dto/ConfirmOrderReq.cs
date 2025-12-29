using System.ComponentModel.DataAnnotations;
using ProductManagement.Order.Enum;

namespace ProductManagement.Order.Dto;

public class ConfirmOrderReq
{
    [Required]
    public Guid OrderId{get;set;}
    [Required]
    [StringLength(maximumLength: 100)]
    public String Address { get; set; }
    [Required]
    [StringLength(maximumLength: 100)]
    public String ReceiverName { get; set; }
    [Required]
    [Phone(ErrorMessage = "Invalid Phone Number")]
    public String ReceiverNumber { get; set; }
    [Required]
    [StringLength(maximumLength: 400)]
    public String TransactionId { get; set; }
    [Required]
    public PaymentMedia Media { get; set; }
    
    
    [StringLength(maximumLength: 500)]
    public String? Note { get; set; }
    
}