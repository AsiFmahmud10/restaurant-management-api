namespace ProductManagement.Config;

public class AuthSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string AccessTokenSecretKey { get; set; }
    public string RefreshTokenSecretKey { get; set; }
    public double AccessTokenExpirationMinutes { get; set; }
    public double RefreshTokenExpirationMinutes { get; set; }
}

