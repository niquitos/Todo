using Microsoft.AspNetCore.Mvc;
using MediatR;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Domain;

namespace AgileBoard.Adapters.WebApi.Controllers;

[ApiController]
[Route("api/sprints")]
public class SprintController : ControllerBase
{
    private readonly IMediator _mediator;

    public SprintController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SprintDto>>> GetSprints(CancellationToken cancellationToken)
    {
        var query = new GetSprintsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SprintDto>> GetSprintById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetSprintByIdQuery(new SprintId(id));
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SprintDto>> CreateSprint([FromBody] CreateSprintDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateSprintCommand(dto);
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetSprintById), new { id }, new SprintDto(id, dto.Name, dto.StartDate, dto.EndDate, dto.Description, false));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateSprint(Guid id, [FromBody] UpdateSprintDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdateSprintCommand(new SprintId(id), dto);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteSprint(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSprintCommand(new SprintId(id));
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
