namespace Website.Api.Features.IdentityManagement.Models;

public record ProfileResp(string Id, UserInfo UserInfo, IList<string> Roles);