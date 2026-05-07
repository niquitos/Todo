using Microsoft.EntityFrameworkCore;
using AgileBoard.Domain;

namespace AgileBoard.Adapters.Persistence.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly AppDbContext _context;

    public TaskItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(TaskItemId id, CancellationToken cancellationToken = default)
    {
        return await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetBySprintIdAsync(SprintId sprintId, CancellationToken cancellationToken = default)
    {
        return await _context.TaskItems
            .Where(t => t.SprintId == sprintId)
            .OrderBy(t => t.ColumnType)
            .ThenBy(t => t.Position)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        await _context.TaskItems.AddAsync(taskItem, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        _context.TaskItems.Update(taskItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TaskItemId id, CancellationToken cancellationToken = default)
    {
        var taskItem = await GetByIdAsync(id, cancellationToken);
        if (taskItem != null)
        {
            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetMaxPositionAsync(SprintId sprintId, ColumnType columnType, CancellationToken cancellationToken = default)
    {
        return await _context.TaskItems
            .Where(t => t.SprintId == sprintId && t.ColumnType == columnType)
            .MaxAsync(t => (int?)t.Position, cancellationToken) ?? -1;
    }
}
