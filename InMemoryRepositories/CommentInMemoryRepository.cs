namespace InMemoryRepositories;
using Entities;
using RepositoryContracts;

public class CommentInMemoryRepository : ICommentRepository
{


    private List<Comment> _comments = new()
    {
        new Comment { Id = 1, Body = "bbb", UserId = 2, PostId = 1 },
        new Comment { Id = 2, Body = "eeee", UserId = 1, PostId = 1 },
        new Comment { Id = 3, Body = "eeee", UserId = 3, PostId = 2 }
    };
    
    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = _comments.Any() 
            ? _comments.Max(c => c.Id) + 1
            : 1;
        _comments.Add(comment);
        return Task.FromResult(comment);
    }
    
    public Task UpdateAsync(Comment comment)
    {
        Comment? existingComment = _comments.SingleOrDefault(c => c.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{comment.Id}' not found");
        }

        _comments.Remove(existingComment);
        _comments.Add(comment);

        return Task.CompletedTask;
    }
    
    public Task DeleteAsync(int id)
    {
        Comment? commentToRemove = _comments.SingleOrDefault(c => c.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        _comments.Remove(commentToRemove);
        return Task.CompletedTask;
    }

    public Task<Comment?> GetSingleAsync(int id)
    {
        Comment? comment = _comments.SingleOrDefault(c => c.Id == id);
        return Task.FromResult(comment);
    }

    public IQueryable<Comment> GetManyAsync()
    {
        return _comments.AsQueryable();
    }
    
}