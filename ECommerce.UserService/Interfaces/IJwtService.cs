using ECommerce.UserService.Models;

namespace ECommerce.UserService.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}