using MediatR;
using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Domain;

namespace AgileBoard.Application.UseCases.Sprints;

public record GetSprintByIdQuery(SprintId Id) : IRequest<SprintDto>;

public class GetSprintByIdQueryHandler : IRequestHandler<GetSprintByIdQuery, SprintDto>
{
    private readonly ISprintRepository _repository;

    public GetSprintByIdQueryHandler(ISprintRepository repository)
    {
        _repository = repository;
    }

    public async Task<SprintDto> Handle(GetSprintByIdQuery request, CancellationToken cancellationToken)
    {
        var sprint = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Спринт не найден.");

        return SprintDto.FromSprint(sprint);
    }
}
