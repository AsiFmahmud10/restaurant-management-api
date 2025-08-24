namespace ProductManagement.Auth.Dto;

public class ResetPasswordReq
{
    public string Token { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}