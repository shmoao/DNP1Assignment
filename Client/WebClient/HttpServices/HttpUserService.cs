using ApiContracts;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebClient.HttpServices;

public class HttpUserService : IUserService
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions Opts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpUserService(HttpClient client)
    {
        _client = client;
    }

    public async Task<UserDto> AddAsync(CreateUserDto request)
    {
        var resp = await _client.PostAsJsonAsync("users", request);
        var json = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode) throw new Exception(json);
        return JsonSerializer.Deserialize<UserDto>(json, Opts)!;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync()
    {
        return await _client.GetFromJsonAsync<IReadOnlyList<UserDto>>("users")
               ?? Array.Empty<UserDto>();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        return await _client.GetFromJsonAsync<UserDto>($"users/{id}");
    }
}