using System.Threading.Tasks;

namespace WebClient.Auth;

public interface IAuthService
{
    Task LoginAsync(string userName, string password);
    Task LogoutAsync();
}