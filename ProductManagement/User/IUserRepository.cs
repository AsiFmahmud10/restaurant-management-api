using ProductManagement.Db;

namespace ProductManagement.User;

public interface IUserRepository : IGenericDbOperation<User>
{
    User? FindByEmail(string email);
    User? GetUserWithToken(string email);
}