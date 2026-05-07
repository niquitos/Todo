using MediatR;
using AgileBoard.Adapters.Persistence.Repositories;

namespace AgileBoard.Application.UseCases.Sprints;

public record GetSprintsQuery : IRequest<IEnumerable<SprintDto>>;

public class GetSprintsQueryHandler : IRequestHandler<GetSprintsQuery, IEnumerable<SprintDto>>
{
    private readonly ISprintRepository _repository;

    public GetSprintsQueryHandler(ISprintRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SprintDto>> Handle(GetSprintsQuery request, CancellationToken cancellationToken)
    {
        var sprints = await _repository.GetAllAsync(cancellationToken);
        return sprints.Select(SprintDto.FromSprint);
    }
}
