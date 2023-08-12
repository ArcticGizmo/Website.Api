using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Client.Model;
using System.Security.Claims;
using Microsoft.Net.Http.Headers;
using System.Data;
using Website.Api.Features.IdentityManagement.Models;

namespace Website.Api.Features.IdentityManagement.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly OpenFgaClient _fga;
    private readonly IHttpClientFactory _httpFactory;
    private readonly string _userInfoEndpoint;
    public ProfileController(OpenFgaClient fga, IHttpClientFactory httpFactory, IConfiguration config)
    {
        _fga = fga;
        _httpFactory = httpFactory;
        _userInfoEndpoint = $"https://{config["Auth0:Domain"]}/userinfo";
    }

    [HttpGet]
    public async Task<ProfileResp> Get(CancellationToken ct)
    {
        var token = Request.Headers[HeaderNames.Authorization].ToString();

        var roleTask = GetRoles(User, ct);
        var userInfoTask = GetUserInfo(token, ct);

        await Task.WhenAll(roleTask, userInfoTask);

        var roles = roleTask.Result;
        var userInfo = userInfoTask.Result;

        return new ProfileResp(userInfo.sub ?? "", userInfo, roles);
    }

    private async Task<IList<string>> GetRoles(ClaimsPrincipal user, CancellationToken ct = default)
    {
        var userId = user.Identity?.Name;
        if (userId == null)
            return Array.Empty<string>();

        var req = new ClientListObjectsRequest()
        {
            User = $"user:{userId}",
            Relation = "assigned",
            Type = "role"
        };
        var resp = await _fga.ListObjects(req, null, ct);

        return resp.Objects!.Select(r => r.Replace("role:", "")).ToList();
    }

    private async Task<UserInfo> GetUserInfo(string bearerToken, CancellationToken ct)
    {
        var client = _httpFactory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.Authorization, bearerToken);
        var resp = await client.GetAsync(_userInfoEndpoint, ct);
        return (await resp.Content.ReadFromJsonAsync<UserInfo>())!;
    }

}