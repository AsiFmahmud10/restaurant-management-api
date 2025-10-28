using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductManagement.User;

[Route("api/v1/user")]
public class UserController(IUserService userService) : Controller
{
    [Authorize(Roles = "admin")]
    [HttpPost("/{userId}/role/{roleId}")]
    [SwaggerOperation("Assign role to a User",description:"Assign role to User")]
    public IActionResult AddRole(Guid userId, Guid roleId)
    {
        return Ok(userService.AddRole(userId, roleId));
    }
    
    [Authorize(Roles = "admin")]
    [HttpDelete("/{userId}/role/{roleId}")]
    [SwaggerOperation("Remove role from a User",description:"Remove role from User")]
    public IActionResult RemoveRole(Guid userId, Guid roleId)
    {
        return Ok(userService.RemoveRole(userId, roleId));
    }
    
}