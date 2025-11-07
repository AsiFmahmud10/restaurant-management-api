using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Product.Dto;

public class CreateProductRequest
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(20,ErrorMessage = "Product name cannot exceed 20 characters")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Product description is required")]
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public required string Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public required decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public required int Quantity { get; set; }

    [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
    public int Rating { get; set; } = 0;

    [Required(ErrorMessage = "Product code is required")]
    public required int Code { get; set; }

    public bool Stock { get; set; } = true;

    public string? Tags { get; set; }

    [Required(ErrorMessage = "Category ID is required")]
    public required Guid CategoryId { get; set; }
}