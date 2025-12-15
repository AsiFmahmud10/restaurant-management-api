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
        var existedCategory = categoryRepository.FindById(categoryId);
        if (existedCategory is null)
        {
            throw new ResourceNotFoundException("Category not exist");
        }

        if (categoryRepository.CategoryHasProducts(categoryId))
        {
            throw new BadRequestException("Products Exist for this Category");
        }
        categoryRepository.Delete(existedCategory);
    }

    public Category? FindById(Guid categoryId)
    {
        return categoryRepository.FindById(categoryId);
    }

    public List<GetCategoryResponse> GetCategory()
    {
       return categoryRepository.GetAll().Select(category => new GetCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
        }).ToList();
        
    }
}