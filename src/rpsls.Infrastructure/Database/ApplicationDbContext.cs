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
        
        modelBuilder.Entity<GameResult>(builder =>
        {
            builder.ToTable("results");
            builder.HasKey(ar => ar.Id);
            builder.Property(ar => ar.Id).ValueGeneratedOnAdd();
        });        
        
        modelBuilder.Entity<ArchivedGameResult>(builder =>
        {
            builder.ToTable("archived_results");
            builder.HasKey(ar => ar.Id);
            builder.Property(ar => ar.Id) .ValueGeneratedOnAdd();
        });
    }
}

