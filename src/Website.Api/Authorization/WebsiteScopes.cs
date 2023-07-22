namespace Website.Api.Authorization;

public static class WbesiteScopes
{
    // users and IAM
    public const string ManageIdentity = "manage:users";

    // library
    public const string ReadLibrary = "read:library";
    public const string WriteLibrary = "write:library";

    public static string[] All() => new string[] {
        ManageIdentity,
        ReadLibrary,
        WriteLibrary
    };
}