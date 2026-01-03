using ProductManagement.Db;

namespace ProductManagement.Category;

public class CategoryRepository(ApplicationDbContext dbContext) : GenericDbOperation<Category>(dbContext),ICategoryRepository
{
    public bool CategoryHasProducts(Guid categoryId)
    {
        return dbContext.Products.Any(p => p.CategoryId.Equals(categoryId));
    }
}