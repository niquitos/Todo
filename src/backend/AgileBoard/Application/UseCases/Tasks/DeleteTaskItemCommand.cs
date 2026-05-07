using MediatR;
using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Domain;

namespace AgileBoard.Application.UseCases.Tasks;

public record DeleteTaskItemCommand(TaskItemId TaskId, SprintId SprintId) : IRequest;

public class DeleteTaskItemCommandHandler : IRequestHandler<DeleteTaskItemCommand>
{
    private readonly ITaskItemRepository _repository;

    public DeleteTaskItemCommandHandler(ITaskItemRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteTaskItemCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.TaskId, cancellationToken)
            ?? throw new NotFoundException("Задача не найдена.");

        await _repository.DeleteAsync(request.TaskId, cancellationToken);

        var remainingTasks = (await _repository.GetBySprintIdAsync(request.SprintId, cancellationToken))
            .Where(t => t.ColumnType == task.ColumnType && t.Position > task.Position)
            .ToList();

        foreach (var remaining in remainingTasks)
        {
            remaining.Move(remaining.ColumnType, remaining.Position - 1);
            await _repository.UpdateAsync(remaining, cancellationToken);
        }
    }
}
