using MediatR;
using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Domain;

namespace AgileBoard.Application.UseCases.Sprints;

public record UpdateSprintCommand(SprintId Id, UpdateSprintDto Dto) : IRequest;

public class UpdateSprintCommandHandler : IRequestHandler<UpdateSprintCommand>
{
    private readonly ISprintRepository _repository;

    public UpdateSprintCommandHandler(ISprintRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateSprintCommand request, CancellationToken cancellationToken)
    {
        var sprint = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Спринт не найден.");

        foreach (var existingSprint in await _repository.GetAllAsync(cancellationToken))
        {
            if (existingSprint.Id.Value != request.Id.Value &&
                existingSprint.OverlapsWith(request.Dto.StartDate, request.Dto.EndDate))
            {
                throw new SprintOverlapException("Диапазон дат пересекается с существующим спринтом.");
            }
        }

        sprint.Update(request.Dto.Name, request.Dto.StartDate, request.Dto.EndDate, request.Dto.Description);
        await _repository.UpdateAsync(sprint, cancellationToken);
    }
}
