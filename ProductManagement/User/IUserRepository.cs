using System.Linq.Expressions;
using ProductManagement.Db;

namespace ProductManagement.User;

public interface IUserRepository : IGenericDbOperation<User>
{
    User? FindByEmail(string email, params Expression<Func<User, object>>[] includes);
    User? GetUserWithToken(string email);
}