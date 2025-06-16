namespace ProductManagement.entity;
using Token; 
public class User : BaseEntity
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public ICollection<Token>? Tokens { get; set; }
    public Order? Order { get; set; }
    public Cart? Cart { get; set; }

    public string getFullName()
    {
        return $"{FirstName} {LastName}";
    } 
    
    public virtual ICollection<Role> Roles { get;} = new List<Role>();
}

 
