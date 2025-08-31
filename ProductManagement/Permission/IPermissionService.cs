namespace ProductManagement.Permission;

public interface IPermissionService
{
    Permission? FindById(Guid id);
}