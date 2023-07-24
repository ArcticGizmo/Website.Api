using System.Text.Json.Serialization;

namespace Website.Api.Models;
public class Auth0ManagementTokenResp
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("expires_in")]

    public required int ExpiresIn { get; init; }
}