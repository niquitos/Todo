using Microsoft.EntityFrameworkCore;
using AgileBoard.Domain;
using AgileBoard.Adapters.Persistence.Configurations;

namespace AgileBoard.Adapters.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Sprint> Sprints => Set<Sprint>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new SprintConfiguration());
        modelBuilder.ApplyConfiguration(new TaskItemConfiguration());
    }
}
