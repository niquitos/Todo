using Microsoft.AspNetCore.Mvc;
using MediatR;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Application.UseCases.Tasks;
using AgileBoard.Domain;

namespace AgileBoard.Adapters.WebApi.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskItemController : ControllerBase
{
    private readonly IMediator _mediator;

    public TaskItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTaskItems(Guid sprintId, CancellationToken cancellationToken)
    {
        var query = new GetTaskItemsQuery(new SprintId(sprintId));
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItemDto>> CreateTaskItem([FromBody] CreateTaskItemDto dto, CancellationToken cancellationToken)
    {
        var effectiveSprintId = dto.SprintId;
        if (!effectiveSprintId.HasValue)
        {
            // Use first sprint if not specified
            var sprints = await _mediator.Send(new GetSprintsQuery(), cancellationToken);
            if (sprints.Any())
            {
                effectiveSprintId = sprints.First().Id;
            }
        }
        var command = new CreateTaskItemCommand(new SprintId(effectiveSprintId!.Value), dto);
        var id = await _mediator.Send(command, cancellationToken);
        var result = new TaskItemDto(id, dto.Name, dto.Description, effectiveSprintId.Value, dto.ColumnType, 0);
        return Created(string.Empty, result);
    }

    [HttpPut("{taskId:guid}")]
    public async Task<ActionResult> UpdateTaskItem(Guid taskId, [FromBody] UpdateTaskItemDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdateTaskItemCommand(new TaskItemId(taskId), dto);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<ActionResult> DeleteTaskItem(Guid taskId, CancellationToken cancellationToken)
    {
        var command = new DeleteTaskItemCommand(new TaskItemId(taskId), null);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

}
