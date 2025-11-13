using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    private readonly JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true
    };
    private async Task<List<User>> ReadAllAsync()
    {
        if (!File.Exists(filePath))
        {
            return new List<User>();
        }

        await using FileStream fs = File.OpenRead(filePath);
        var users = await JsonSerializer.DeserializeAsync<List<User>>(fs, jsonOptions);
        return users ?? new List<User>();
    }

    private async Task WriteAllAsync(List<User> users)
    {
        await using FileStream fs = File.Create(filePath);
        await JsonSerializer.SerializeAsync(fs, users, jsonOptions);
    }
    
    public async Task<User> AddAsync(User user)
    {
        var users = await ReadAllAsync();
        user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
        users.Add(user);
        await WriteAllAsync(users);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        var users = await ReadAllAsync();
        int index = users.FindIndex(u => u.Id == user.Id);
        if (index == -1)
            throw new InvalidOperationException($"User with ID '{user.Id}' not found");

        users[index] = user;
        await WriteAllAsync(users);
    }

    public async Task DeleteAsync(int id)
    {
        var users = await ReadAllAsync();
        int removed = users.RemoveAll(u => u.Id == id);
        if (removed == 0)
            throw new InvalidOperationException($"User with ID '{id}' not found");

        await WriteAllAsync(users);
    }

    public async Task<User?> GetSingleAsync(int id)
    {
        var users = await ReadAllAsync();
        return users.SingleOrDefault(u => u.Id == id);
    }

    public IQueryable<User> GetManyAsync()
    {
        if (!File.Exists(filePath))
        {
            return new List<User>().AsQueryable();
        }

        string json = File.ReadAllText(filePath);
        var users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        return users.AsQueryable();
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        var users = await ReadAllAsync();
        return users.SingleOrDefault(
            u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
    }
}
