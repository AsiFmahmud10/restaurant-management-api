using ProductManagement.Db;

namespace ProductManagement.Category;

public class CategoryRepository(ApplicationDbContext dbContext) : GenericDbOperation<Category>(dbContext),ICategoryRepository
{
   
}