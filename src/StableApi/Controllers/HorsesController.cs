namespace StableApi.Controllers;

using StableApi.Models;
using StableApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HorsesController : ControllerBase
{
    private readonly IHorseService _service;

    public HorsesController(IHorseService service) => _service = service;

    [HttpGet]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest("Invalid pagination parameters.");
        }

        return Ok(_service.GetPaged(page, pageSize));
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var horse = _service.GetById(id);
        // ISSUE: returns 200 with a null body when horse does not exist
        return Ok(horse);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateHorseRequest request)
    {
        // ISSUE: manual validation — should be handled by FluentValidation
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length < 2)
        {
            return BadRequest("Name must be at least 2 characters.");
        }

        if (!request.OwnerEmail.Contains('@'))
        {
            return BadRequest("Owner email is invalid.");
        }

        var horse = _service.Create(request);
        // ISSUE: should return 201 Created with a Location header
        return Ok(horse);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateHorseRequest request)
    {
        var horse = _service.Update(id, request);
        if (horse is null)
        {
            return NotFound();
        }

        return Ok(horse);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _service.Delete(id);
        if (!deleted)
        {
            return NotFound();
        }

        // ISSUE: should return 204 NoContent
        return Ok();
    }
}
