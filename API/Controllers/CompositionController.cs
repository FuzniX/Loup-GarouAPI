using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CompositionController(CompositionService compositionService) : BaseApiController
{
    [HttpPost("new")]
    public IActionResult NewComposition([FromBody] CompositionCreationRequest request)
    {
        try
        {
            compositionService.CreateComposition(request);
            return Created();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("all")]
    public IActionResult GetCompositions() => Ok(compositionService.GetCompositions());
}