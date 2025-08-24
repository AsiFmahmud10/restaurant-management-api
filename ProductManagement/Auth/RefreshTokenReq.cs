using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Auth;

public class RefreshTokenReq
{
    [Required(ErrorMessage = "Refresh Token is required")]
    public string RefreshToken { get; set; }
}