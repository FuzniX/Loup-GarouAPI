using System.ComponentModel.DataAnnotations;

namespace Data;

public class Composition
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string Description { get; set; }
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    
    public override string ToString() => $"{Name} : {Description}\n" + string.Join("\n", Roles.Select(role => $"- {role}"));
}