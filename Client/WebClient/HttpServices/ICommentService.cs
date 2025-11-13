using ApiContracts;

namespace WebClient.HttpServices;

public interface ICommentService
{
    Task<IReadOnlyList<CommentDto>> GetAllAsync();
    Task<CommentDto?> GetByIdAsync(int id);
    Task<CommentDto> AddAsync(CreateCommentDto request);
    Task DeleteAsync(int id);
}