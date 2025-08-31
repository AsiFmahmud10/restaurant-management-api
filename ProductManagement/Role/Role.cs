namespace ProductManagement.Role;

using Db;
using Permission;

public class Role : BaseEntity
{
    public string Name { get; set; }
    public ICollection<Permission> Permissions { get; } = new HashSet<Permission>();
}