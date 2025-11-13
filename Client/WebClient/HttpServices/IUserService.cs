using ApiContracts;

namespace WebClient.HttpServices;

public interface IUserService
{
    Task<UserDto> AddAsync(CreateUserDto request);
    Task<IReadOnlyList<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id);
}