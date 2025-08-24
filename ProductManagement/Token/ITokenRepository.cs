using ProductManagement.Db;

namespace ProductManagement.Token;

public interface ITokenRepository : IGenericDbOperation<Token>
{
    Token? GetByValueWithUser(string token);
    void RemoveTokenByUserId(Guid userId);
    
    void RemoveTokenByValue(string token);
}