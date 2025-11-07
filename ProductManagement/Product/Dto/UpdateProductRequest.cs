using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Product.Dto;

public class UpdateProductRequest
{
    [StringLength(20,ErrorMessage = "Product name cannot exceed 20 characters")]
    public string? Name { get; set; }
    
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public string? Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal? Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int? Quantity { get; set; }

    [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
    public int? Rating { get; set; } = 0;

    public int? Code { get; set; }

    public bool? Stock { get; set; } = true;

    public string? Tags { get; set; }
}