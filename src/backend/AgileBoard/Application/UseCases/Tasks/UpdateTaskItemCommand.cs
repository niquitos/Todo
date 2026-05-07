using MediatR;
using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Sprints;
using AgileBoard.Domain;

namespace AgileBoard.Application.UseCases.Tasks;

public record UpdateTaskItemCommand(TaskItemId TaskId, SprintId SprintId, UpdateTaskItemDto Dto) : IRequest;

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

        task.Update(request.Dto.Name, request.Dto.Description);
        await _repository.UpdateAsync(task, cancellationToken);
    }
}
