using ECommerce.UserService.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.UserService.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string HashPassword(string password)
        {
            return _hasher.HashPassword(null!, password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            var result = _hasher.VerifyHashedPassword(
                null!,
                passwordHash,
                password
            );

            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}