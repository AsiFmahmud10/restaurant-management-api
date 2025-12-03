using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Db;

namespace ProductManagement.User;

public class UserRepository(ApplicationDbContext dbContext) : GenericDbOperation<User>(dbContext),IUserRepository 
{
    public User? FindByEmail(string email,params Expression<Func<User,object>>[] includes)
    {
        IQueryable<User> query = dbContext.Users;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }
       
        return query.FirstOrDefault(user => user.Email.Equals(email));
    }

    public User? GetUserWithToken(string email)
    {
        return dbContext.Users.Include(user => user.Tokens)
            .FirstOrDefault(u => u.Email == email);
    }

    public User? FindUserWithCartDetails(Guid userId)
    {
        return dbContext.Users.Include(user => user.Cart).ThenInclude((cart => cart.CartItems))
            .SingleOrDefault(user => user.Id.Equals(userId));
    }
}