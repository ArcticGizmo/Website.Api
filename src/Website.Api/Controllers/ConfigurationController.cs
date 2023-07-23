using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization;
using Fga.Net.AspNetCore.Authorization.Attributes;
using Website.Api.Authorization;

namespace Website.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfiguration _config;
    public ConfigurationController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    [AllowAnonymous]
    public string Get()
    {
        var a = _config["ClientUrl"];
        return a ?? "not set";
    }

    [HttpGet("bacon")]
    public string GetBacon()
    {
        var a = User;
        return "bacon";
    }

    [HttpGet("admin")]
    [Authorize("manage:users")]
    [Authorize(FgaAuthorizationDefaults.PolicyKey)]
    [FgaRole(FgaRoles.Admin)]
    public string GetAdmin()
    {
        return "admin";
    }

    [HttpGet("book-editor")]
    public string GetBookEditor()
    {
        return "book editor";
    }
}