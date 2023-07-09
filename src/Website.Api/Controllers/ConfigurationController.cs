using Microsoft.AspNetCore.Mvc;

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
    public string Get()
    {
        var a = _config["ConnectionStrings:MongoDB"];
        return a ?? "not set";
    }
}