using ProductManagement.Exception;
using ProductManagement.Role;
using ProductManagement.Role.Dto;

namespace ProductManagement.Permission;

public class PermissionService(IPermissionRepository permissionRepository) : IPermissionService
{
    public Permission? FindById(Guid id)
    {
        return permissionRepository.FindById(id);
    }

    
}