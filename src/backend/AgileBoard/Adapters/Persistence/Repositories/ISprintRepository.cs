using AgileBoard.Domain;

namespace AgileBoard.Adapters.Persistence.Repositories;

public interface ISprintRepository
{
    Task<Sprint?> GetByIdAsync(SprintId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Sprint>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Sprint?> GetDefaultAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Sprint sprint, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sprint sprint, CancellationToken cancellationToken = default);
    Task DeleteAsync(SprintId id, CancellationToken cancellationToken = default);
}
