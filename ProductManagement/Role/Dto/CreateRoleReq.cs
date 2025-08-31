using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Role.Dto;

public class CreateRoleReq
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    [Required]
    public List<int> Permissions { get; set; }
}