using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Auth;
using ProductManagement.Common.Annotation;
using ProductManagement.Role.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductManagement.Role;

[Route("api/roles")]
public class RoleController(IRoleService roleService) : Controller
{
    [AllowAnonymous]
    [HttpPost("/create")]
    [SwaggerOperation("Create a new role", description: "Create a new role")]
    public IActionResult CreateRole([FromBody] CreateRoleReq createRoleReq)
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest();
        }

        roleService.Create(createRoleReq);
        return Ok("User registered successfully");
    }

    [AllowAnonymous]
    [HttpPost("/{roleId}/{permissionId}")]
    [SwaggerOperation("Add permission to Role", description: "Add permission to Role")]
    public IActionResult AddPermission([FromBody] AddPermissionReq addPermissionReq, Guid roleId)
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest();
        }

        roleService.AddPermission(addPermissionReq, roleId);
        return Ok("Permission added successfully");
    }
}