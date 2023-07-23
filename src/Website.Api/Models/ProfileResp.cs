namespace Website.Api.Models;

public record ProfileResp(string Id, UserInfo UserInfo, IList<string> Roles);