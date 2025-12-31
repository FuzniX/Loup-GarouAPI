using System.ComponentModel.DataAnnotations;

namespace Data;

public class Group
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string Description { get; set; }
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
    
    public override string ToString() => $"{Name} : {Description}" + Players.Select(player => $"- {player}\n");
}