namespace Fga.Net.AspNetCore.Authorization.Attributes;

/// <summary>
/// RBAC check attribute
/// </summary>
public class FgaRoleAttribute : FgaBaseObjectAttribute
{
    private const string Relation = "assigned";
    private readonly string _object;

    public FgaRoleAttribute(string role)
    {
        _object = $"role:{role}";
    }

    public override ValueTask<string> GetRelation(HttpContext context) => ValueTask.FromResult(Relation);
    public override ValueTask<string> GetObject(HttpContext context) => ValueTask.FromResult(_object);
}