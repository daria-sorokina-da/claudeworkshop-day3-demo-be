namespace StableApi.Controllers;

using FluentValidation;
using StableApi.Models;
using StableApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HorsesController : ControllerBase
{
    private readonly IHorseService _service;
    private readonly IValidator<CreateHorseRequest> _createValidator;
    private readonly IValidator<RetireHorseRequest> _retireValidator;

    public HorsesController(IHorseService service, IValidator<CreateHorseRequest> createValidator, IValidator<RetireHorseRequest> retireValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _retireValidator = retireValidator;
    }

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
        var validation = _createValidator.Validate(request);
        if (!validation.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));
        }

        var horse = _service.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = horse.Id }, horse);
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

    [HttpPost("{id}/retire")]
    public IActionResult Retire(int id, [FromBody] RetireHorseRequest request)
    {
        var validation = _retireValidator.Validate(request);
        if (!validation.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

        if (_service.GetById(id) is null) return NotFound();

        _service.Retire(id, request);
        return StatusCode(201);
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
