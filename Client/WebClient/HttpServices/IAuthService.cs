using System.Threading.Tasks;

namespace WebClient.HttpServices;

public interface IAuthService
{
    Task LoginAsync(string userName, string password);
    Task LogoutAsync();
}