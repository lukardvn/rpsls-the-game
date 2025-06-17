using Microsoft.EntityFrameworkCore;
using rpsls.Domain.Models;

namespace rpsls.Infrastructure.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<GameResult> Results => Set<GameResult>();
    public DbSet<ArchivedGameResult> ArchivedResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<GameResult>().ToTable("results");
        modelBuilder.Entity<ArchivedGameResult>().ToTable("archived_results");
    }
}

