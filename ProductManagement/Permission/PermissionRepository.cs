using ProductManagement.Db;

namespace ProductManagement.Permission;

public class PermissionRepository(ApplicationDbContext context) : GenericDbOperation<Permission>(context), IPermissionRepository
{
   
    public Permission? FindByName(string name)
    {
        return this.Find(permission => permission.Name.Equals(name)).FirstOrDefault();
    }
}