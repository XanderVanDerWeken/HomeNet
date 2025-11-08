namespace HomeNet.Infrastructure.Persistence;

using HomeNet.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

public class HomeDbContext : DbContext
{
    public DbSet<ChoreEntity> Chores { get; set; }
    public DbSet<ChoreSeriesEntity> ChoreSeries { get; set; }

    public HomeDbContext(DbContextOptions<HomeDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChoreSeriesEntity>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<ChoreEntity>()
            .HasKey(c => c.Id);
    }
}
