using MediatR;
using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Domain;

namespace AgileBoard.Application.UseCases.Tasks;

public record MoveTaskItemCommand(TaskItemId TaskId, SprintId SprintId, ColumnType NewColumnType, int NewPosition) : IRequest;

public class MoveTaskItemCommandHandler : IRequestHandler<MoveTaskItemCommand>
{
    private readonly ITaskItemRepository _repository;

    public MoveTaskItemCommandHandler(ITaskItemRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(MoveTaskItemCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.TaskId, cancellationToken)
            ?? throw new NotFoundException("Задача не найдена.");

        var sprintId = request.SprintId;
        var oldColumnType = task.ColumnType;
        var oldPosition = task.Position;

        if (oldColumnType == request.NewColumnType)
        {
            if (request.NewPosition < oldPosition)
            {
                var tasksToShift = (await _repository.GetBySprintIdAsync(sprintId, cancellationToken))
                    .Where(t => t.ColumnType == oldColumnType
                        && t.Id != task.Id
                        && t.Position >= request.NewPosition
                        && t.Position < oldPosition)
                    .ToList();

                foreach (var t in tasksToShift)
                {
                    t.Move(oldColumnType, t.Position + 1);
                    await _repository.UpdateAsync(t, cancellationToken);
                }
            }
            else if (request.NewPosition > oldPosition)
            {
                var tasksToShift = (await _repository.GetBySprintIdAsync(sprintId, cancellationToken))
                    .Where(t => t.ColumnType == oldColumnType
                        && t.Id != task.Id
                        && t.Position > oldPosition
                        && t.Position <= request.NewPosition)
                    .ToList();

                foreach (var t in tasksToShift)
                {
                    t.Move(oldColumnType, t.Position - 1);
                    await _repository.UpdateAsync(t, cancellationToken);
                }
            }

            task.Move(request.NewColumnType, request.NewPosition);
            await _repository.UpdateAsync(task, cancellationToken);
        }
        else
        {
            // Remove from source column: shift tasks below down
            var sourceTasksToShift = (await _repository.GetBySprintIdAsync(sprintId, cancellationToken))
                .Where(t => t.ColumnType == oldColumnType && t.Position > oldPosition)
                .ToList();

            foreach (var t in sourceTasksToShift)
            {
                t.Move(oldColumnType, t.Position - 1);
                await _repository.UpdateAsync(t, cancellationToken);
            }

            // Insert into target column: shift tasks at target position and below up
            var targetTasksToShift = (await _repository.GetBySprintIdAsync(sprintId, cancellationToken))
                .Where(t => t.ColumnType == request.NewColumnType && t.Position >= request.NewPosition)
                .ToList();

            foreach (var t in targetTasksToShift)
            {
                t.Move(request.NewColumnType, t.Position + 1);
                await _repository.UpdateAsync(t, cancellationToken);
            }

            task.Move(request.NewColumnType, request.NewPosition);
            await _repository.UpdateAsync(task, cancellationToken);
        }
    }
}
