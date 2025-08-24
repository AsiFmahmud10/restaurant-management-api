using ProductManagement.Auth.Dto;

namespace ProductManagement.Auth;

public interface IAuthService
{
    void Register(UserRegisterReq registerReq);
    LoginResponse Login(LoginReq loginReq);
    LoginResponse RefreshToken(RefreshTokenReq refreshTokenReq);
    void ChangePassword(ChangePasswordReq changePasswordReq);
    void ForgetPassword(ForgetPasswordReq forgetPasswordReq);
    void ResetPassword(string token, ResetPasswordReq resetPasswordReq);
}