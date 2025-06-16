namespace ProductManagement.entity;

public class Category : BaseEntity
{
    public string Name {get; set;}
    public virtual ICollection<Product> Products {get;} =  new List<Product>();
}