using System.ComponentModel.DataAnnotations;

namespace Data;

public abstract class Nameable
{
    [Key] public int Id { get; init; }
    [Required] public required string Name { get; init; }

    public override string ToString() => $"{Name}";
}

// public class Phase : Nameable
// {
//     public virtual ICollection<Role> Roles { get; init; } = new List<Role>();
// }
//
// public class Camp : Nameable
// {
//     public virtual ICollection<Role> Roles { get; init; } = new List<Role>();
// }

public class Player : Nameable
{
    public virtual ICollection<Group> Groups { get; init; } = new List<Group>();
}

public class Role : Nameable
{
    public required string ImageUrl { get; init; }
    public required int DefaultPriority { get; init; }
    public required Camp Camp { get; init; }
    public virtual ICollection<RolePhase> Phases { get; init; } = [];
    public virtual ICollection<Composition> Compositions { get; init; } = [];
}