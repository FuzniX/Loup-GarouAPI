using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class GroupController(GroupService groupService) : BaseApiController
{
    [HttpPost("new")]
    public IActionResult NewGroup([FromBody] GroupCreationRequest request)
    {
        try
        {
            groupService.CreateGroup(request);
            return Created();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("all")]
    public IActionResult GetGroups() => Ok(groupService.GetGroups());
}