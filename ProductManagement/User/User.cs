using ProductManagement.Db;

namespace ProductManagement.User;
using Token; 
using Cart;
using Role; 
using Order;
public class User : BaseEntity
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public ICollection<Token> Tokens { get;} = new List<Token>();
    public  ICollection<Order> Orders{ get; set; } = new List<Order>();
    public Cart? Cart { get; set; }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    public Order AddOrder(Order order)
    {
        order.User = this;
        this.Orders.Add(order);
        return order;
    }
    
    public virtual ICollection<Role> Roles { get;} = new List<Role>();
}

 
