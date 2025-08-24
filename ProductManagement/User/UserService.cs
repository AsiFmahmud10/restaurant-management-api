namespace ProductManagement.User;

public class UserService(IUserRepository userRepository) : IUserService
{
    public User? FindByEmail(string email)
    {
        return userRepository.FindByEmail(email);
    }
    
    public User? FindById(Guid id)
    {
        return userRepository.FindById(id);
    }

    public User? FindByPassword(string existedHashedPassword)
    {
        return userRepository.FirstOrDefault(user => user.Password == existedHashedPassword);
    }

    public User? GetUserWithToken(string email)
    {
        return userRepository.GetUserWithToken(email);
    }

    public void Update(User user)
    {
        userRepository.Update(user);
    }

    public void Save(User user)
    {
        userRepository.save(user);
    }
}