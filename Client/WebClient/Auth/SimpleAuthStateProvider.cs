using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using ApiContracts;   

namespace WebClient.Auth;

public class SimpleAuthStateProvider : AuthenticationStateProvider
{
    private UserDto? currentUser;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsIdentity identity;

        if (currentUser is null)
        {
            identity = new ClaimsIdentity();
        }
        else
        {
            identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString()),
                new Claim(ClaimTypes.Name, currentUser.UserName)
            }, "simple");
        }

        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult(new AuthenticationState(principal));
    }

    public Task UpdateUserAsync(UserDto? user)
    {
        currentUser = user;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return Task.CompletedTask;
    }
}