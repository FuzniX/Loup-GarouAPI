using System.ComponentModel.DataAnnotations;

namespace Data;

public abstract class Descriptible : Nameable
{
    [Required] public required string Description { get; init; }

    public override string ToString() => $"{Name} : {Description}";
}

public class Group : Descriptible
{
    public virtual ICollection<Player> Players { get; init; } = new List<Player>();

    public override string ToString() => $"{base.ToString()}\n {string.Join("\n", Players.Select(player => $"- {player}"))}";
}

public class Composition : Descriptible
{
    public virtual ICollection<Role> Roles { get; init; } = new List<Role>();

    public override string ToString() => $"{base.ToString()}\n {string.Join("\n", Roles.Select(role => $"- {role}"))}";
}

public class Role : Descriptible
{
    public required string ImageUrl { get; init; }
    public required int DefaultPriority { get; init; }
    public required Camp Camp { get; init; }
    public required Phase Phase { get; init; }
    public virtual ICollection<Composition> Compositions { get; init; } = new List<Composition>();
}