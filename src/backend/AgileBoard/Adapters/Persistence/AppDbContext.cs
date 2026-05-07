using Microsoft.EntityFrameworkCore;

namespace AgileBoard.Adapters.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
