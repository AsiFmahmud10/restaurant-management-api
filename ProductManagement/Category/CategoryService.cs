using ProductManagement.Category.Dto;
using ProductManagement.Exception;

namespace ProductManagement.Category;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public void CreateCategory(CreateCategoryRequest request)
    {
        var existedCategory = categoryRepository.Find(category => category.Name.Equals(request.Name)).FirstOrDefault();
        if (existedCategory is not null)
        {
            throw new BadRequestException("Category already exists");
        }
        var newCategory = new Category
        {
            Name = request.Name,
        };
        categoryRepository.save(newCategory);
    }

    public void DeleteCategory(Guid categoryId)
    {
        var existedCategory =  categoryRepository.FindById(categoryId);
        if (existedCategory is null)
        {
            throw new ResourceNotFoundException("Category not exist");
        }
        categoryRepository.Delete(existedCategory);
    }
}