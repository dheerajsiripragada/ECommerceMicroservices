using ECommerce.UserService.Interfaces;
using ECommerce.UserService.Models;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task AddAsync(User user)
        {
            var existingUser =
                await _userRepository.GetByEmailAsync(user.Email);

            if (existingUser != null)
                throw new Exception("Email already registered.");

            user.PasswordHash =
                _passwordHasher.HashPassword(user.PasswordHash);

            await _userRepository.AddAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user =
                await _userRepository.GetByEmailAsync(email);

            if (user == null)
                return null;

            var isPasswordValid =
                _passwordHasher.VerifyPassword(
                    password,
                    user.PasswordHash
                );

            if (!isPasswordValid)
                return null;

            return user;
        }
    }
}