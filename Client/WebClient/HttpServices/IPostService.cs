using ApiContracts;

namespace WebClient.HttpServices;

public interface IPostService
{
    Task<PostDto> AddAsync(CreatePostDto request);
    Task<IReadOnlyList<PostDto>> GetAllAsync();
    Task<PostDto?> GetByIdAsync(int id);
}