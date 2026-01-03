using Microsoft.EntityFrameworkCore;

namespace Data;

public class LgDbContext(DbContextOptions<LgDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Composition> Compositions { get; set; }
    public DbSet<Group> Groups { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>().Property(r => r.Camp).HasConversion<string>();
        modelBuilder.Entity<Role>().Property(r => r.Phase).HasConversion<string>();
    }
}
