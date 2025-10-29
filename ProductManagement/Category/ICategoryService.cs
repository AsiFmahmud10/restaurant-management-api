using ProductManagement.Category.Dto;

namespace ProductManagement.Category;

public interface ICategoryService 
{
    void CreateCategory(CreateCategoryRequest request);
    void DeleteCategory(Guid categoryId);
}