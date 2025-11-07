using ProductManagement.Db;

namespace ProductManagement.Category;

public interface ICategoryRepository : IGenericDbOperation<Category>
{
    public bool CategoryHasProducts(Guid categoryId);
}