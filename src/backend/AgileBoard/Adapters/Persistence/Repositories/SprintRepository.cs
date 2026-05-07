using Microsoft.EntityFrameworkCore;
using AgileBoard.Domain;

namespace AgileBoard.Adapters.Persistence.Repositories;

public class SprintRepository : ISprintRepository
{
    private readonly AppDbContext _context;

    public SprintRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Sprint?> GetByIdAsync(SprintId id, CancellationToken cancellationToken = default)
    {
        return await _context.Sprints
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Sprint>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sprints.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Sprint sprint, CancellationToken cancellationToken = default)
    {
        await _context.Sprints.AddAsync(sprint, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Sprint sprint, CancellationToken cancellationToken = default)
    {
        _context.Sprints.Update(sprint);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(SprintId id, CancellationToken cancellationToken = default)
    {
        var sprint = await GetByIdAsync(id, cancellationToken);
        if (sprint != null)
        {
            _context.Sprints.Remove(sprint);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
