using api.Models;
using Microsoft.EntityFrameworkCore;

namespace tidybee_hub.Context;

public class DatabaseContext : DbContext
{
    public DbSet<AgentModel> Agents { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AgentModel>()
            .HasKey(e => e.Uuid);

        base.OnModelCreating(modelBuilder);
    }
}
