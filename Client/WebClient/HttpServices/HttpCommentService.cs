using ApiContracts;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebClient.HttpServices;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions Opts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpCommentService(HttpClient client)
    {
        _client = client;
    }

    public async Task<CommentDto> AddAsync(CreateCommentDto request)
    {
        var resp = await _client.PostAsJsonAsync("comments", request);
        var json = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode) throw new Exception(json);
        return JsonSerializer.Deserialize<CommentDto>(json, Opts)!;
    }

    public async Task<IReadOnlyList<CommentDto>> GetAllAsync()
    {
        return await _client.GetFromJsonAsync<IReadOnlyList<CommentDto>>("comments")
               ?? Array.Empty<CommentDto>();
    }

    public async Task<CommentDto?> GetByIdAsync(int id)
    {
        return await _client.GetFromJsonAsync<CommentDto>($"comments/{id}");
    }
}