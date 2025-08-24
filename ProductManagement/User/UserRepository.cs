using Microsoft.EntityFrameworkCore;
using ProductManagement.Db;

namespace ProductManagement.User;

public class UserRepository(ApplicationDbContext dbContext) : GenericDbOperation<User>(dbContext),IUserRepository 
{
    public User? FindByEmail(string email)
    {
        return dbContext.Users.FirstOrDefault(u => u.Email == email);
    }

    public User? GetUserWithToken(string email)
    {
        return dbContext.Users.Include(user => user.Tokens)
            .FirstOrDefault(u => u.Email == email);
    }
}