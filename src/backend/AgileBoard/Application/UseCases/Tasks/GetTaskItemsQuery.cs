using MediatR;
using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Domain;

namespace AgileBoard.Application.UseCases.Tasks;

public record GetTaskItemsQuery(SprintId SprintId) : IRequest<IEnumerable<TaskItemDto>>;

public class GetTaskItemsQueryHandler : IRequestHandler<GetTaskItemsQuery, IEnumerable<TaskItemDto>>
{
    private readonly ITaskItemRepository _repository;

    public GetTaskItemsQueryHandler(ITaskItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TaskItemDto>> Handle(GetTaskItemsQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _repository.GetBySprintIdAsync(request.SprintId, cancellationToken);
        return tasks.Select(TaskItemDto.FromTaskItem);
    }
}
