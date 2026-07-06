using UserManagerApi.Models;

namespace UserManagerApi.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user, string role);
    }
}
