namespace ProductManagement.Token;
using ProductManagement.User;

public interface ITokenService
{
    public string? GenerateAccessToken(User user);
    public string? GenerateRefreshToken();
    public Token? GetByValueWithUser(string value);
    bool ValidateToken(string refreshToken);
    void DeleteTokenByUserId(Guid userId);
    Token GenerateResetPasswordToken();
    void DeleteTokenByTokenValue(string existedTokenValue);
}