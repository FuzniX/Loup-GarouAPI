using System.ComponentModel.DataAnnotations;

namespace Data;

public class Player
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    
    public override string ToString() => $"{Name}";
}