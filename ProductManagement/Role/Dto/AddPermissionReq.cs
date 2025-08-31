using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Role.Dto;

public class AddPermissionReq
{
    [Required]
    public Guid PermissionId { get; set; }
}