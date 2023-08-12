using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using Website.Api.Features.IdentityManagement.Models;

namespace Website.Api.Features.IdentityManagement;

public class Auth0ManagementApi : IAuth0ManagementApi
{
    private readonly Uri _endpoint;
    private readonly string _token_endpoint;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _managementClientId;
    private readonly string _managementClientSecret;
    private readonly string _orgId;
    private readonly string _clientId;

    private ManagementApiClient _client;
    private DateTime _refreshAfter = DateTime.MinValue;

    public Auth0ManagementApi(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _managementClientId = config["Auth0Management:ClientId"] ?? throw new ArgumentNullException("managementClientId");
        _managementClientSecret = config["Auth0Management:ClientSecret"] ?? throw new ArgumentNullException("managementClientSecret");
        _orgId = config["Auth0:OrgId"] ?? throw new ArgumentNullException("orgId");
        _clientId = config["Auth0:ClientId"] ?? throw new ArgumentNullException("clientId");

        var domain = config["Auth0:Domain"];
        _token_endpoint = $"https://{domain}/oauth/token";
        _endpoint = new Uri($"https://{domain}/api/v2/");

        _client = new ManagementApiClient("", _endpoint);
    }

    private async Task<ManagementApiClient> GetClient()
    {
        var now = DateTime.UtcNow;
        if (now > _refreshAfter)
        {
            var token = await GetToken();
            _client.UpdateAccessToken(token.AccessToken);
            _refreshAfter = now.AddSeconds(token.ExpiresIn);
        }

        return _client;
    }

    private async Task<Auth0ManagementTokenResp> GetToken()
    {
        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/json");
        using var jsonContent = JsonContent.Create(new
        {
            client_id = _managementClientId,
            client_secret = _managementClientSecret,
            audience = _endpoint.ToString(),
            grant_type = "client_credentials"
        });
        using var resp = await client.PostAsync(_token_endpoint, jsonContent);
        return (await resp.Content.ReadFromJsonAsync<Auth0ManagementTokenResp>())!;
    }

    public Task DeleteUser(string userId)
    {
        throw new NotImplementedException();
    }

    public Task GetUser(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IPagedList<User>> GetUsers()
    {
        var client = await GetClient();
        return await client.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo());
    }

    public async Task<OrganizationInvitation> InviteUser(string email)
    {

        var client = await GetClient();
        var req = new OrganizationCreateInvitationRequest()
        {
            Inviter = new OrganizationInvitationInviter { Name = "Duck Gang Admin" },
            Invitee = new OrganizationInvitationInvitee { Email = email },
            ClientId = _clientId,
        };
        return await client.Organizations.CreateInvitationAsync(_orgId, req);
    }
}