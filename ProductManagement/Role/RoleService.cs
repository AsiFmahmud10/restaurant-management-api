using System.Linq.Expressions;
using ProductManagement.Exception;
using ProductManagement.Role.Dto;

namespace ProductManagement.Role;

using Permission;

public class RoleService(IRoleRepository roleRepository, IPermissionService permissionService) : IRoleService
{
    public void Create(CreateRoleReq createRoleReq)
    {
        var existedRole = roleRepository.FindByName(createRoleReq.Name);

        if (existedRole is null)
        {
            throw new BadHttpRequestException("Role already exists");
        }

        var role = new Role()
        {
            Name = createRoleReq.Name
        };

        roleRepository.save(role);
    }

    public void AddPermission(AddPermissionReq addPermissionReq, Guid roleId)
    {
        var role = roleRepository.FindById(roleId, role => role.Permissions);
        if (role == null)
        {
            throw new ResourceNotFoundException($"Role id : {roleId} not found");
        }

        Permission? requstedPermission = permissionService.FindById(addPermissionReq.PermissionId);

        if (requstedPermission is null)
        {
            throw new ResourceNotFoundException($"Permission id : {addPermissionReq.PermissionId} not found");
        }

        if (!role.Permissions.Contains(requstedPermission))
        {
            role.Permissions.Add(requstedPermission);
            roleRepository.Update(role);
        }
    }

    public Role? FindById(Guid id, params Expression<Func<Role, object>>[] includes)
    {
        return roleRepository.FindById(id, includes);
    }

    public Role? FindByName(string roleName)
    {
        return roleRepository.FindByName(roleName);
    }
}