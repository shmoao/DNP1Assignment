using ApiContracts;

namespace WebClient.HttpServices;

public interface ICommentService
{
    Task<CommentDto> AddAsync(CreateCommentDto request);
    Task<IReadOnlyList<CommentDto>> GetAllAsync();
    Task<CommentDto?> GetByIdAsync(int id);
}