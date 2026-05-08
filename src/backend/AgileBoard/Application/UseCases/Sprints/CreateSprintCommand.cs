using MediatR;
using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Domain;

namespace AgileBoard.Application.UseCases.Sprints;

public record CreateSprintCommand(CreateSprintDto Dto) : IRequest<Guid>;

public class CreateSprintCommandHandler : IRequestHandler<CreateSprintCommand, Guid>
{
    private readonly ISprintRepository _repository;

    public CreateSprintCommandHandler(ISprintRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateSprintCommand request, CancellationToken cancellationToken)
    {
        var allSprints = await _repository.GetAllAsync(cancellationToken);

        foreach (var existingSprint in allSprints)
        {
            if (!existingSprint.IsDefault && existingSprint.OverlapsWith(request.Dto.StartDate, request.Dto.EndDate))
            {
                throw new SprintOverlapException("Диапазон дат пересекается с существующим спринтом.");
            }
        }

        var sprint = Sprint.Create(request.Dto.Name, request.Dto.StartDate, request.Dto.EndDate, request.Dto.Description);
        await _repository.AddAsync(sprint, cancellationToken);

        return sprint.Id.Value;
    }
}
