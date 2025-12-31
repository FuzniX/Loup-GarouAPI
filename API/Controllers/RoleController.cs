using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class RoleController(RoleService roleService) : BaseApiController
{
    [HttpPost("new")]
    public IActionResult NewRole([FromBody] RoleCreationRequest request)
    {
        roleService.CreateRole(request);
        return Created();
    }

    [HttpGet("all")]
    public IActionResult GetAllRoles() => Ok(roleService.GetAllRoles());
}