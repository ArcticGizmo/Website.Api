using Microsoft.AspNetCore.Authorization;

namespace Website.Api.Authorization;

public class ScopeAuthorizationRequirement : IAuthorizationRequirement
{

    public string Issuer { get; }
    public string Scope { get; }
    public ScopeAuthorizationRequirement(string scope, string issuer)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
    }
}