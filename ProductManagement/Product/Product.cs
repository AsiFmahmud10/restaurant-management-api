namespace ProductManagement.Product;
using Db;
using Category;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int Rating { get; set; } = 0;
    public int Code {get; set; }
    public bool Stock {get; set;} =  true;
    public string? Tags {get; set; }

    public Guid CategoryId {get; set;}
    public Category Category { get; set; }
}

