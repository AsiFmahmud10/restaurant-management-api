using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Auth;

public class ForgetPasswordReq
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}