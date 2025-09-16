namespace InMemoryRepositories;
using Entities;
using RepositoryContracts;

public class UserInMemoryRepository : IUserRepository
{


    private List<User> _users = new()
    {
        new User { Id = 1, UserName = "Soma", Password = "soma123" },
        new User { Id = 2, UserName = "Rebeca", Password = "rebeca123" },
        new User { Id = 3, UserName = "Chris", Password = "chris123" }
    };
    
    public Task<User> AddAsync(User user)
    {
        user.Id = _users.Any() 
            ? _users.Max(u => u.Id) + 1
            : 1;
        _users.Add(user);
        return Task.FromResult(user);
    }
    
    public Task UpdateAsync(User user)
    {
        User? existingUser = _users.SingleOrDefault(u => u.Id == user.Id);
        if (existingUser is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{user.Id}' not found");
        }

        _users.Remove(existingUser);
        _users.Add(user);

        return Task.CompletedTask;
    }
    
    public Task DeleteAsync(int id)
    {
        User? userToRemove = _users.SingleOrDefault(u => u.Id == id);
        if (userToRemove is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        _users.Remove(userToRemove);
        return Task.CompletedTask;
    }

    public Task<User> GetSingleAsync(int id)
    {
        User? user = _users.SingleOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    public IQueryable<User> GetManyAsync()
    {
        return _users.AsQueryable();
    }
}