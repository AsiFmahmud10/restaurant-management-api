using ProductManagement.Db;

namespace ProductManagement.Role;

public interface IRoleRepository : IGenericDbOperation<Role>
{
    Role? FindByName(string name);
}