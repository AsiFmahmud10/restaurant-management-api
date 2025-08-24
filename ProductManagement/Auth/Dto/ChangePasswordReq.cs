using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Auth.Dto;

public class ChangePasswordReq
{
    
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public string OldPassword { get; set; }
    [Required(ErrorMessage = "New password is required")]
    public string NewPassword { get; set; }
}