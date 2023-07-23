using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Website.Api.Authorization;
using OpenFga.Sdk.Client;

namespace Website.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(WbesiteScopes.ManageIdentity)]
[Authorize(FgaAuthorizationDefaults.PolicyKey)]
[FgaRole(FgaRoles.Admin)]
public class IdentityManagementController : ControllerBase
{
    private readonly OpenFgaClient _fga;
    public IdentityManagementController(OpenFgaClient fga)
    {
        _fga = fga;
    }

    [HttpGet("users")]
    public async Task GetUsers()
    {
        Console.WriteLine("get all users");
    }

    [HttpGet("users/{id}")]
    public async Task GetUser(string id)
    {
        // how do we manage user permissions?
        Console.WriteLine("Getting specific user");
    }

    // TODO: should this be patch instead?
    [HttpPost("users/{:id}/roles")]
    public async Task UpdateRoles()
    {
        Console.WriteLine("Updating user roles");
        // not sure what happens when creating more than 10 tuples at a time
    }

    [HttpPost("invite-user")]
    public async Task InviteUser()
    {
        // I think we will need to switch to organization flows to restrict users logging in
        Console.WriteLine("Invite User");
    }

    [HttpPost("remove-user")]
    public async Task RemoveUser()
    {
        Console.WriteLine("Remove User");
        // this will be complicated as we also need to remove all permissions within openFga
    }
}