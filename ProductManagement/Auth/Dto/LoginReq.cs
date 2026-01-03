using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Auth.Dto;

public class LoginReq
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress()]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
    public Guid? CartId { get; set; }
}