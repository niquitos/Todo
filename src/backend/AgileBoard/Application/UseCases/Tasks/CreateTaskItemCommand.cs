using MediatR;
using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Domain;

namespace AgileBoard.Application.UseCases.Tasks;

public record CreateTaskItemCommand(SprintId SprintId, CreateTaskItemDto Dto) : IRequest<Guid>;

public class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, Guid>
{
    private readonly ITaskItemRepository _repository;

    public CreateTaskItemCommandHandler(ITaskItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
    {
        var columnType = Enum.Parse<ColumnType>(request.Dto.ColumnType);
        var existingTasks = (await _repository.GetBySprintIdAsync(request.SprintId, cancellationToken))
            .Where(t => t.ColumnType == columnType)
            .ToList();

        foreach (var task in existingTasks)
        {
            task.Move(columnType, task.Position + 1);
            await _repository.UpdateAsync(task, cancellationToken);
        }

        var newTask = TaskItem.Create(request.Dto.Name, request.Dto.Description, request.SprintId, columnType, 0);
        await _repository.AddAsync(newTask, cancellationToken);

        return newTask.Id.Value;
    }
}
