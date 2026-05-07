using MediatR;
using AgileBoard.Adapters.Persistence.Repositories;
using AgileBoard.Domain;

namespace AgileBoard.Application.UseCases.Sprints;

public record DeleteSprintCommand(SprintId Id) : IRequest;

public class DeleteSprintCommandHandler : IRequestHandler<DeleteSprintCommand>
{
    private readonly ISprintRepository _repository;

    public DeleteSprintCommandHandler(ISprintRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteSprintCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id, cancellationToken);
    }
}
