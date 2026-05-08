using Microsoft.AspNetCore.Mvc;
using MediatR;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Application.UseCases.Tasks;
using AgileBoard.Domain;

namespace AgileBoard.Adapters.WebApi.Controllers;

[ApiController]
[Route("api/sprints/{sprintId:guid}/tasks")]
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
    public async Task<ActionResult<TaskItemDto>> CreateTaskItem(Guid sprintId, [FromBody] CreateTaskItemDto dto, CancellationToken cancellationToken)
    {
        var effectiveSprintId = dto.SprintId ?? sprintId;
        var command = new CreateTaskItemCommand(new SprintId(effectiveSprintId), dto);
        var id = await _mediator.Send(command, cancellationToken);
        var result = new TaskItemDto(id, dto.Name, dto.Description, effectiveSprintId, dto.ColumnType, 0);
        return Created(string.Empty, result);
    }

    [HttpPut("{taskId:guid}")]
    public async Task<ActionResult> UpdateTaskItem(Guid sprintId, Guid taskId, [FromBody] UpdateTaskItemDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdateTaskItemCommand(new TaskItemId(taskId), new SprintId(sprintId), dto);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<ActionResult> DeleteTaskItem(Guid sprintId, Guid taskId, CancellationToken cancellationToken)
    {
        var command = new DeleteTaskItemCommand(new TaskItemId(taskId), new SprintId(sprintId));
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPut("move")]
    public async Task<ActionResult> MoveTaskItem(Guid sprintId, [FromBody] MoveTaskItemDto dto, CancellationToken cancellationToken)
    {
        var newColumnType = Enum.Parse<ColumnType>(dto.NewColumnType);
        var command = new MoveTaskItemCommand(new TaskItemId(dto.TaskId), new SprintId(sprintId), newColumnType, dto.NewPosition);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
