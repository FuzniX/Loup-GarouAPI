using System.ComponentModel.DataAnnotations;

namespace Data;

public class Role
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string Description { get; set; }
    public string ImageURL { get; set; }
    public int DefaultPriority { get; set; }
    public virtual ICollection<Composition> Compositions { get; set; } = new List<Composition>();
    
    public override string ToString() => $"{Name}, {Description}";
}