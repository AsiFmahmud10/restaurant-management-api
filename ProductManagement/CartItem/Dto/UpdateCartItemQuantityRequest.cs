using System.ComponentModel.DataAnnotations;

namespace ProductManagement.CartItem.Dto;

public class UpdateCartItemQuantityRequest
{
    [Range(0, 200, ErrorMessage = "Quantity must be between 0 and 200")]
    public int Quantity { get; set; }
}