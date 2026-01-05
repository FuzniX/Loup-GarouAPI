using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class GameController(GameService gameService) : BaseApiController
{
    [HttpPost("new")]
    public IActionResult New([FromBody] GameCreationRequest request)
    {
        try
        {
            var gameId = gameService.CreateGame(request);
            return Accepted(new { GameId = gameId });
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPost("{id}/next")]
    public IActionResult Next(string id, [FromBody] GameMasterRequest request)
    {
        try
        {
            return Accepted(gameService.Next(id, request));
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Game not found.");
        }
    }
}