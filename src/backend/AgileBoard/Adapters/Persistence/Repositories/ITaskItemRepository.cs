using AgileBoard.Domain;

namespace AgileBoard.Adapters.Persistence.Repositories;

public interface ITaskItemRepository
{
    Task<TaskItem?> GetByIdAsync(TaskItemId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskItem>> GetBySprintIdAsync(SprintId sprintId, CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(TaskItemId id, CancellationToken cancellationToken = default);
    Task<int> GetMaxPositionAsync(SprintId sprintId, ColumnType columnType, CancellationToken cancellationToken = default);
}
