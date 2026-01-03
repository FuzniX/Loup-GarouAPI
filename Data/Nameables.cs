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