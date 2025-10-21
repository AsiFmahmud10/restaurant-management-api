using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProductManagement.Config;

namespace ProductManagement.Token;

using System;
using User;

public class TokenService(AuthSettings authSettings,ITokenRepository tokenRepository) : ITokenService
{
    public Token? GetByValueWithUser(string token)
    {
        return tokenRepository.GetByValueWithUser(token);
    }
    
    public bool ValidateToken(string refreshToken)
    {
        var tokenValidator = new JwtSecurityTokenHandler();
        var tokenValidatorParams = new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.RefreshTokenSecretKey)),
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateIssuer = false
        };

        try
        {
            tokenValidator.ValidateToken(refreshToken, tokenValidatorParams, out SecurityToken validatedToken);
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    public void DeleteTokenByUserId(Guid userId)
    {
        tokenRepository.RemoveTokenByUserId(userId);
    }

    public Token GenerateResetPasswordToken()
    {
        byte[] randomBytes = new byte[12];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        var tokenValue = Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        Token token = new Token()
        {
            Value = tokenValue,
            ValidUntil = DateTime.UtcNow.AddMinutes(10),
            TokenType = TokenType.Reset
        };

        return token;
    }

    public void DeleteTokenByTokenValue(string existedTokenValue)
    {
        tokenRepository.RemoveTokenByValue(existedTokenValue);
    }
    
    public string? GenerateAccessToken(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim("userId", user.Id.ToString()),
            new Claim("email", user.Email),
        };
        
        return  this.GenerateToken(
            authSettings.AccessTokenSecretKey,
            authSettings.Issuer,
            authSettings.Audience,
            authSettings.AccessTokenExpirationMinutes,
            claims
        );
    }
    
    public string? GenerateRefreshToken()
    {
        return this.GenerateToken(
            authSettings.RefreshTokenSecretKey,
            authSettings.Issuer,
            authSettings.Audience,
            authSettings.RefreshTokenExpirationMinutes
        );
    }

   
    private string? GenerateToken(string key, string issuer, string audience, double expire,
        IEnumerable<Claim>? claims = null)
    {
        SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        SigningCredentials credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken tokenObj = new JwtSecurityToken(
            issuer, audience, claims,
            DateTime.UtcNow, DateTime.UtcNow.AddHours(expire),
            credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenObj);
    }

}