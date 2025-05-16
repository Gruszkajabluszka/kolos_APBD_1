using kolos_apbd_v2.Exceptions;
using kolos_apbd_v2.Models;
using kolos_apbd_v2.Services;
using Microsoft.AspNetCore.Mvc;

namespace kolos_apbd_v2.Controllers;
[Route("api/[controller]")]
[ApiController]
public class VisitsController :ControllerBase
{
    private readonly IVisitServices _visitService;

    public VisitsController(IVisitServices visitServices)
    {
        _visitService = visitServices;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVisit(string id)
    {
        try
        {
            var res = await _visitService.GetVisits(id);
            return Ok(res);
        }
        catch(NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    [HttpPost]
    public async Task<IActionResult> AddNewVisit([FromBody] PVisitsDTO postVisit)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _visitService.PostVisits(postVisit);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException exception)
        {
            return Conflict(exception.Message);
        }


        return Created("", "Utworzono");
    }
}
