using ProductManagement.Db;

namespace ProductManagement.Product;

public class Category : BaseEntity
{
    public string Name {get; set;}
    public virtual ICollection<ProductManagement.Product.Product> Products {get;} =  new List<ProductManagement.Product.Product>();
}