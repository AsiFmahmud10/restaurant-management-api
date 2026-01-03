namespace ProductManagement.Order.Dto;

public class OrderRequest
{
    public List<Items> Items {get; set;} = new();
}

public class Items
{
    public Guid ProductId {get;set;}
    public int Quantity {get;set;}
    public decimal PriceAtPurchaseTime {get;set;}
}