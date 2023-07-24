using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using Website.Api.Models;

namespace Website.Api.Services;

public class Auth0ManagementApi : IAuth0ManagementApi
{
    private readonly Uri _endpoint;
    private readonly string _token_endpoint;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _clientId;
    private readonly string _clientSecret;

    private ManagementApiClient _client;
    private DateTime _refreshAfter = DateTime.MinValue;

    public Auth0ManagementApi(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _clientId = config["Auth0Management:ClientId"] ?? throw new ArgumentNullException("clientId");
        _clientSecret = config["Auth0Management:ClientSecret"] ?? throw new ArgumentNullException("clientSecret");

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
            client_id = _clientId,
            client_secret = _clientSecret,
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
        var users = await client.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo());
        return users;
    }

    public Task InviteUser(string email)
    {
        throw new NotImplementedException();
    }
}