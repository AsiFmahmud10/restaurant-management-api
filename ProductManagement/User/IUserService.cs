using ProductManagement.Db;

namespace ProductManagement.User;

public interface IUserService 
{
    void Save(User user);
    User? FindByEmail(string email);
    
    User? GetUserWithToken(string email);
    void Update(User user);
    User? FindById(Guid tokenUserId);
    User? FindByPassword(string existedHashedPassword);
}