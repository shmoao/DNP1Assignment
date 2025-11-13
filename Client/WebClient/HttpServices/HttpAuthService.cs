using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ApiContracts;         
using WebClient.Auth;

namespace WebClient.HttpServices;

public class HttpAuthService : IAuthService
{
    private readonly HttpClient httpClient;
    private readonly SimpleAuthStateProvider authStateProvider;

    public HttpAuthService(HttpClient httpClient, SimpleAuthStateProvider authStateProvider)
    {
        this.httpClient = httpClient;
        this.authStateProvider = authStateProvider;
    }

    public async Task LoginAsync(string userName, string password)
    {
        var request = new LoginRequest
        {
            UserName = userName,
            Password = password
        };

        var response = await httpClient.PostAsJsonAsync("auth/login", request);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Login failed");

        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        if (user is null)
            throw new Exception("Invalid user data from server");

        await authStateProvider.UpdateUserAsync(user);
    }

    public async Task LogoutAsync()
    {
        await authStateProvider.UpdateUserAsync(null);
    }
}