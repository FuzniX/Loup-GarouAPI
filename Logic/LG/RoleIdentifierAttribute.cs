namespace Logic.LG;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RoleIdentifierAttribute(string roleName, Camp camp, Phase phase) : Attribute
{
    public string RoleName { get; } = roleName;
    public Camp Camp { get; } = camp;
    public Phase Phase { get; } = phase;
}