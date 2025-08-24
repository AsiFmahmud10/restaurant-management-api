using ProductManagement.Db;

namespace ProductManagement.User;
using Token; 
using Cart;
using Role; 
public class User : BaseEntity
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public ICollection<Token> Tokens { get;} = new List<Token>();
    public Order.Order? Order { get; set; }
    public Cart? Cart { get; set; }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    } 
    
    public virtual ICollection<Role> Roles { get;} = new List<Role>();
}

 
