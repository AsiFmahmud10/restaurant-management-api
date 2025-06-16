using ProductManagement.entity;

namespace ProductManagement.Token;

public class Token : BaseEntity
{
    public Guid UserId { get; set; }
    public string Value { get; set; }
    public TokenType TokenType { get; set; }
}

