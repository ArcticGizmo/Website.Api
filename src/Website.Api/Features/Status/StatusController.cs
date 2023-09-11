using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Website.Api.Features.Status.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class StatusController : ControllerBase
{
    [HttpGet]
    public ActionResult GetStatus() => Ok();
}