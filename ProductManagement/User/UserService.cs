using System.Linq.Expressions;
using ProductManagement.Exception;

namespace ProductManagement.User;

using Role;

public class UserService(IUserRepository userRepository, IRoleService roleService) : IUserService
{
    public User? FindById(Guid id, params Expression<Func<User, object?>>[] includes)
    {
        return userRepository.FindById(id, includes);
    }

    public User? FindByPassword(string existedHashedPassword)
    {
        return userRepository.FirstOrDefault(user => user.Password == existedHashedPassword);
    }

    public string AddRole(Guid userId, Guid roleId)
    {
        var user = userRepository.FindById(userId, user => user.Roles);
        if (user is null)
        {
            throw new ResourceNotFoundException("User not found");
        }

        var requestedRole = roleService.FindById(roleId);
        if (requestedRole is null)
        {
            throw new ResourceNotFoundException("Role not found");
        }

        if (user.Roles.Any(role => role.Name.Equals(requestedRole.Name)))
        {
            throw new BadRequestException("Role already exists");
        }

        user.Roles.Add(requestedRole);
        userRepository.Update(user);

        return "Role added Successfully";
    }

    public string RemoveRole(Guid userId, Guid roleId)
    {
        var user = userRepository.FindById(userId, user => user.Roles);
        if (user is null)
        {
            throw new ResourceNotFoundException("User not found");
        }

        var requestedRole = roleService.FindById(roleId);
        if (requestedRole is null)
        {
            throw new ResourceNotFoundException("Role not found");
        }

        if (user.Roles.Any(role => role.Name.Equals("admin")) && requestedRole.Name.Equals("admin"))
        {
            throw new BadRequestException("Role [admin] of Admin can not be removed");
        }

        if (user.Roles.FirstOrDefault(existedRole => existedRole.Name.Equals(requestedRole.Name)) is { } role)
        {
            user.Roles.Remove(role);
            userRepository.Update(user);
            return "Role removed Successfully";
        }

        throw new BadRequestException("This role doesn't assigned to the user");
    }

    public User? FindUserWithCartDetails(Guid userId)
    {
        return userRepository.FindUserWithCartDetails(userId);
    }

    public User? FindByEmail(string email, params Expression<Func<User, object>>[] includes)
    {
        return userRepository.FindByEmail(email, includes);
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