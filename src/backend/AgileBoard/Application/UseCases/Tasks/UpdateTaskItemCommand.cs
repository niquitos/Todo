using MediatR;
using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Domain;

namespace AgileBoard.Application.UseCases.Tasks;

public record UpdateTaskItemCommand(TaskItemId TaskId, UpdateTaskItemDto Dto) : IRequest;

public class UpdateTaskItemCommandHandler : IRequestHandler<UpdateTaskItemCommand>
{
    private readonly ITaskItemRepository _repository;

    public UpdateTaskItemCommandHandler(ITaskItemRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateTaskItemCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.TaskId, cancellationToken)
            ?? throw new NotFoundException("Задача не найдена.");

        // Update all properties from DTO
        task.Update(request.Dto.Name, request.Dto.Description);
        var columnType = Enum.Parse<ColumnType>(request.Dto.ColumnType);
        task.Move(columnType, request.Dto.Position);
        if (request.Dto.SprintId.HasValue)
        {
            task.ReassignTo(new SprintId(request.Dto.SprintId.Value));
        }

        await _repository.UpdateAsync(task, cancellationToken);
    }
}
