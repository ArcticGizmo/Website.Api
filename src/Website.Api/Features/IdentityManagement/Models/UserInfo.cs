namespace Website.Api.Features.IdentityManagement.Models;

public class UserInfo
{
    public string? sub { get; init; }
    public string? name { get; init; }

    public string? email { get; init; }
}