using ProductManagement.Db;
namespace ProductManagement.Token;
using User;
public class Token : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Value { get; set; }
    
    public DateTime? ValidUntil {get;set;}
    public TokenType TokenType { get; set; }
}

