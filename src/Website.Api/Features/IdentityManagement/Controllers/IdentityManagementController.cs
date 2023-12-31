using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Website.Api.Authorization;
using OpenFga.Sdk.Client;
using Auth0.ManagementApi.Paging;
using Auth0.ManagementApi.Models;

namespace Website.Api.Features.IdentityManagement;

[ApiController]
[Route("[controller]")]
//[Authorize(WbesiteScopes.ManageIdentity)]
//[Authorize(FgaAuthorizationDefaults.PolicyKey)]
//[FgaRole(FgaRoles.Admin)]
public class IdentityManagementController : ControllerBase
{
    private readonly OpenFgaClient _fga;
    private readonly IAuth0ManagementApi _management;
    public IdentityManagementController(OpenFgaClient fga, IAuth0ManagementApi management)
    {
        _fga = fga;
        _management = management;
    }

    [HttpGet("users")]
    public async Task<IPagedList<User>> GetUsers()
    {
        return await _management.GetUsers();
    }

    [HttpGet("users/{id}")]
    public async Task GetUser(string id)
    {
        // how do we manage user permissions?
        Console.WriteLine("Getting specific user");
        await Task.Delay(1);
    }

    // TODO: should this be patch instead?
    [HttpPost("users/{:id}/roles")]
    public async Task UpdateRoles()
    {
        Console.WriteLine("Updating user roles");
        // not sure what happens when creating more than 10 tuples at a time
        await Task.Delay(1);
    }

    [HttpPost("invite-user")]
    public async Task<OrganizationInvitation> InviteUser(string email) => await _management.InviteUser(email);

    [HttpPost("remove-user")]
    public async Task RemoveUser()
    {
        Console.WriteLine("Remove User");
        // this will be complicated as we also need to remove all permissions within openFga
        await Task.Delay(1);
    }
}