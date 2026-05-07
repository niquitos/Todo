using MediatR;
using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Application.UseCases.Tasks;
using AgileBoard.Domain;

namespace AgileBoard.Application.UseCases.Sprints;

public record DeleteSprintCommand(SprintId Id) : IRequest;

public class DeleteSprintCommandHandler : IRequestHandler<DeleteSprintCommand>
{
    private readonly ISprintRepository _sprintRepository;
    private readonly ITaskItemRepository _taskItemRepository;

    public DeleteSprintCommandHandler(ISprintRepository sprintRepository, ITaskItemRepository taskItemRepository)
    {
        _sprintRepository = sprintRepository;
        _taskItemRepository = taskItemRepository;
    }

    public async Task Handle(DeleteSprintCommand request, CancellationToken cancellationToken)
    {
        var sprint = await _sprintRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sprint == null)
            throw new NotFoundException("Спринт не найден.");

        if (sprint.IsDefault)
            throw new InvalidOperationException("Нельзя удалить системный спринт «Не запланировано».");

        var defaultSprint = await _sprintRepository.GetDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("Системный спринт не найден.");

        var tasks = await _taskItemRepository.GetBySprintIdAsync(request.Id, cancellationToken);
        foreach (var task in tasks)
        {
            task.ReassignTo(defaultSprint.Id);
            await _taskItemRepository.UpdateAsync(task, cancellationToken);
        }

        await _sprintRepository.DeleteAsync(request.Id, cancellationToken);
    }
}
