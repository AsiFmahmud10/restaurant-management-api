using ProductManagement.Db;

namespace ProductManagement.Category;
using Product;

public class Category : BaseEntity
{
    public string Name {get; set;}
    public virtual ICollection<Product> Products {get;} =  new List<Product>();
}