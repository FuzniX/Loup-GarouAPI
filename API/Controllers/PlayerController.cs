using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PlayerController(PlayerService playerService) : BaseApiController
{
    [HttpPost("new")]
    public IActionResult NewRole([FromBody] PlayerCreationRequest request)
    {
        playerService.CreatePlayer(request);
        return Created();
    }

    [HttpGet("all")]
    public IActionResult GetAllPlayers() => Ok(playerService.GetAllPlayers());
}