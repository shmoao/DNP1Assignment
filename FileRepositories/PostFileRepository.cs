using RepositoryContracts;

namespace FileRepositories;

using Entities;
using System.Text.Json;

public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";
    
    public PostFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<Post> AddAsync(Post post)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        int maxId = posts.Count > 0 ? posts.Max(c => c.Id) : 1;
        post.Id = maxId + 1;
        posts.Add(post);
        postsAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postsAsJson);
        return post;
    }

    public async Task DeleteAsync(int id)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        
        Post? postToDelete = posts.FirstOrDefault(c => c.Id == id);
        if (postToDelete != null)
        {
            posts.Remove(postToDelete);
            postsAsJson = JsonSerializer.Serialize(posts);
            await File.WriteAllTextAsync(filePath, postsAsJson);
        }
    }

    public async Task<Post?> GetSingleAsync(int id)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        return posts.FirstOrDefault(c => c.Id == id);
    }

    public IQueryable<Post> GetManyAsync()
    {
        string postsAsJson = File.ReadAllTextAsync(filePath).Result;
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        return posts.AsQueryable();
    }

    public async Task UpdateAsync(Post post)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        
        int index = posts.FindIndex(c => c.Id == post.Id);
        if (index != -1)
        {
            posts[index] = post;
            postsAsJson = JsonSerializer.Serialize(posts);
            await File.WriteAllTextAsync(filePath, postsAsJson);
        }
    }
}