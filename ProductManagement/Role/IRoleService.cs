using System.Linq.Expressions;
using ProductManagement.Role.Dto;

namespace ProductManagement.Role;

public interface IRoleService
{
    void Create(CreateRoleReq createRoleReq);
    void AddPermission(AddPermissionReq addPermissionReq, Guid roleId);
    Role? FindById(Guid id, params Expression<Func<Role, object>>[] includes);
    Role? FindByName(string roleName);
}