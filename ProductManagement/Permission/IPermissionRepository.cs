using System.Linq.Expressions;
using ProductManagement.Db;

namespace ProductManagement.Permission;

public interface IPermissionRepository : IGenericDbOperation<Permission>
{
    Permission? FindById(Guid id,params Expression<Func<Permission,object>>[] includes);
    Permission? FindByName(string name);
    
}