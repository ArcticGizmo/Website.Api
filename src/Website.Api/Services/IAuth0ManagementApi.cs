using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;

namespace Website.Api.Services;

public interface IAuth0ManagementApi
{

    public Task GetUser(string userId);
    public Task<IPagedList<User>> GetUsers();

    public Task InviteUser(string email);

    public Task DeleteUser(string userId);
}