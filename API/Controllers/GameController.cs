using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class GameController(GameService gameService) : BaseApiController
{
    [HttpPost("new")]
    public IActionResult NewGame([FromBody] GameCreationRequest request)
    {
        try
        {
            var gameId = gameService.CreateGame(request);
            return Ok(new { GameId = gameId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Game not found.");
        }
    }
    
    [HttpPost("{id}/next")]
    public IActionResult NextStep(string id, [FromBody] GameMasterRequest request)
    {
        try
        {
            var response = gameService.Next(id, request);
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Game not found.");
        }
    }
}