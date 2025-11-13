using System.Net.Http.Json;
using ApiContracts;

namespace WebClient.HttpServices;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient _client;

    public HttpCommentService(HttpClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<CommentDto>> GetAllAsync()
    {
        var result = await _client.GetFromJsonAsync<IReadOnlyList<CommentDto>>("comments");
        return result ?? Array.Empty<CommentDto>();
    }

    public async Task<CommentDto?> GetByIdAsync(int id)
    {
        return await _client.GetFromJsonAsync<CommentDto>($"comments/{id}");
    }

    public async Task<CommentDto> AddAsync(CreateCommentDto request)
    {
        var response = await _client.PostAsJsonAsync("comments", request);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<CommentDto>();
        if (created is null)
            throw new Exception("Failed to deserialize created comment.");

        return created;
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _client.DeleteAsync($"comments/{id}");
        response.EnsureSuccessStatusCode();
    }
}