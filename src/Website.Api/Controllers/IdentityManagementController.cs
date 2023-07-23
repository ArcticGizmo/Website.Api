using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Website.Api.Authorization;
using OpenFga.Sdk.Client;

namespace Website.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class IdentityManagementController : ControllerBase
{
    private readonly OpenFgaClient _fga;
    public IdentityManagementController(OpenFgaClient fga)
    {
        _fga = fga;
    }
}