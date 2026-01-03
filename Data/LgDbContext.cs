using Microsoft.EntityFrameworkCore;

namespace Data;

public class LgDbContext(DbContextOptions<LgDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles { get; set; }
    public DbSet<Camp> Camps { get; set; }
    public DbSet<Phase> Phases { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Composition> Compositions { get; set; }
    public DbSet<Group> Groups { get; set; }
}
