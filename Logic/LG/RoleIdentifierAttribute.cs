namespace Logic.LG;

[AttributeUsage(AttributeTargets.Class)]
public class RoleIdentifierAttribute(string roleName) : Attribute
{
    public string RoleName { get; } = roleName;
}