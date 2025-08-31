using ProductManagement.Db;

namespace ProductManagement.Role;

public class RoleRepository(ApplicationDbContext context) : GenericDbOperation<Role>(context), IRoleRepository
{
    public Role? FindByName(string name)
    {
        return this.Find(role => role.Name.Equals(name)).FirstOrDefault();
    }
}