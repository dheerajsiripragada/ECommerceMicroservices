using ECommerce.UserService.DTOs;
using ECommerce.UserService.Interfaces;
using ECommerce.UserService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UserController(
            IUserService userService,
            IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();

            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            }).ToList();

            return Ok(userDtos);
        }

        [Authorize]
        [HttpGet("get-user-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var currentUserId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var currentUserRole =
                User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole != "Admin" && currentUserId != id)
                return Forbid();

            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(userDto);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            try
            {
                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    PasswordHash = dto.Password,
                    Role = "User"
                };

                await _userService.AddAsync(user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("update-user/{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateUserDto dto)
        {
            var currentUserId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var currentUserRole =
                User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole != "Admin" && currentUserId != id)
                return Forbid();

            var existingUser =
                await _userService.GetByIdAsync(id);

            if (existingUser == null)
                return NotFound();

            if (dto.Name != null)
                existingUser.Name = dto.Name;

            if (dto.Email != null)
                existingUser.Email = dto.Email;

            await _userService.UpdateAsync(existingUser);

            var userDto = new UserDto
            {
                Id = existingUser.Id,
                Name = existingUser.Name,
                Email = existingUser.Email,
                Role = existingUser.Role
            };

            return Ok(userDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            await _userService.DeleteAsync(id);

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userService.LoginAsync(
                dto.Email,
                dto.Password
            );

            if (user == null)
                return Unauthorized("Invalid email or password");

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token,
                user = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role
                }
            });
        }
    }
}