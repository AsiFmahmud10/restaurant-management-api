using Microsoft.EntityFrameworkCore;
using ProductManagement.Db;

namespace ProductManagement.Token;

public class TokenRepository(ApplicationDbContext dbContext) : GenericDbOperation<Token>(dbContext),ITokenRepository
{
    public Token? GetByValueWithUser(string tokenValue)
    {
        return dbContext.Token.Include(token => token.User)
            .FirstOrDefault(token => token.Value == tokenValue);
    }
    public void RemoveTokenByUserId(Guid userId)
    {
        dbContext.Token.Where(token => token.UserId == userId)
            .ExecuteDelete();
    }

    public void RemoveTokenByValue(string value)
    {
        dbContext.Token.Where(token => token.Value.Equals(value))
            .ExecuteDelete();
    }
    
}