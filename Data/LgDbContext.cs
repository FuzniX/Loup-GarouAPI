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
        
        modelBuilder.Entity<RolePhase>().ToTable("RolePhases").HasKey(rp => new { rp.RoleId, rp.Phase });
        modelBuilder.Entity<Role>().HasMany(r => r.Phases).WithOne().HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<Role>().Property(r => r.Camp).HasConversion<string>();
        modelBuilder.Entity<RolePhase>().Property(rp => rp.Phase).HasConversion<string>();
    }
}
