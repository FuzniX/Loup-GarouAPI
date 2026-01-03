namespace Logic.LG;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RoleIdentifierAttribute(string roleName) : Attribute
{
    public string RoleName { get; } = roleName;
}