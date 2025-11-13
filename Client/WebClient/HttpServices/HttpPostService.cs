using ApiContracts;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebClient.HttpServices;

public class HttpPostService : IPostService
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions Opts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpPostService(HttpClient client)
    {
        _client = client;
    }

    public async Task<PostDto> AddAsync(CreatePostDto request)
    {
        var resp = await _client.PostAsJsonAsync("posts", request);
        var json = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode) throw new Exception(json);
        return JsonSerializer.Deserialize<PostDto>(json, Opts)!;
    }

    public async Task<IReadOnlyList<PostDto>> GetAllAsync()
    {
        return await _client.GetFromJsonAsync<IReadOnlyList<PostDto>>("posts")
               ?? Array.Empty<PostDto>();
    }

    public async Task<PostDto?> GetByIdAsync(int id)
    {
        return await _client.GetFromJsonAsync<PostDto>($"posts/{id}");
    }
}