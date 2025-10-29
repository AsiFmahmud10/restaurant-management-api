using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Category.Dto;

public class CreateCategoryRequest
{
    [Required(ErrorMessage = "Category name is required")]
    public string Name { get; set; }
}