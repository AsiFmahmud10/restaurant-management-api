using System.Linq.Expressions;
using ProductManagement.Db;

namespace ProductManagement.User;

public interface IUserService 
{
    void Save(User user);
    User? FindByEmail(string email, params Expression<Func<User, object>>[] includes);
    void Update(User user);
    User? FindById(Guid tokenUserId);
    User? FindByPassword(string existedHashedPassword);
    string AddRole(Guid userId, Guid roleId);
    string RemoveRole(Guid userId, Guid roleId);
}