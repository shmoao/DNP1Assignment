namespace InMemoryRepositories;
using Entities;
using RepositoryContracts;

public class PostInMemoryRepository : IPostRepository
{
    private List<Post> _posts = new()
    {
        new Post { Id = 1, Title = "AAA", Body = "...", UserId = 1 },
        new Post { Id = 2, Title = "BBB", Body = "...", UserId = 2 },
        new Post { Id = 3, Title = "CCC", Body = "...", UserId = 1 }
    };
    
    public Task<Post> AddAsync(Post post)
    {
        post.Id = _posts.Any() 
            ? _posts.Max(p => p.Id) + 1
            : 1;
        _posts.Add(post);
        return Task.FromResult(post);
    }
    
    public Task UpdateAsync(Post post)
    {
        Post? existingPost = _posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{post.Id}' not found");
        }

        _posts.Remove(existingPost);
        _posts.Add(post);

        return Task.CompletedTask;
    }
    
    public Task DeleteAsync(int id)
    {
        Post? postToRemove = _posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        _posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        Post? post = _posts.SingleOrDefault(p => p.Id == id);
        return Task.FromResult(post);
    }

    public IQueryable<Post> GetManyAsync()
    {
        return _posts.AsQueryable();
    }
    

    
}