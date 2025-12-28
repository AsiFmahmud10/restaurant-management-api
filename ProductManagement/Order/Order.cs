using ProductManagement.Order.Enum;

namespace ProductManagement.Order;

using User;
using Db;

public class Order : BaseEntity
{
    public OrderStatus Status { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Identifier { get; set; }

    public string? Address { get; set; }
    public string? Note { get; set; }
    public string? ReceieverName { get; set; }
    public string? ReceiverNumber { get; set; }
    public Payment? Payment { get; set; }
    public string? ShipmentTrackingUrl { get; set; }

    public void AddIdentifier()
    {
        this.Identifier = Guid.CreateVersion7().ToString().Substring(0, 9);
    }

    public virtual ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

    public decimal GetTotalPrice()
    {
        return OrderItems.Sum(item => item.GetTotalPrice());
    }
    public void Confirm(string address, Payment payment,string receiverNumber, string? note = null)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new ApplicationException("Only Pending Order can be confirmed");
        }

        Address = address;
        Payment = payment;
        payment.Order = this;
        this.Note = note;
        ReceiverNumber = receiverNumber;

        ArgumentException.ThrowIfNullOrWhiteSpace(this.Address, "Invalid Address");
        ArgumentNullException.ThrowIfNull(this.Payment, "Payment null");
        ArgumentException.ThrowIfNullOrWhiteSpace(this.ReceiverNumber, "Invalid ReceiverNumber");

        this.Status = OrderStatus.Confirmed;
    }
}