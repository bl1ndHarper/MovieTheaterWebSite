using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.DTOs;
using MovieTheater.Application.Interfaces;
using MovieTheater.Infrastructure.Entities;
using MovieTheater.Infrastructure.Interfaces;
using System.Security.Claims;

namespace MovieTheater.Application.Services
{

    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;

        public AccountService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IHttpContextAccessor httpContext,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContext = httpContext;
            _userRepository = userRepository;
        }

        public async Task<(bool Success, string? Error)> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return (false, "bad creds");

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                return (false, "bad creds");

            await SignInUserAsync(user);
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> RegisterAsync(RegisterRequestDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return (false, "email exists");

            var userName = string.IsNullOrWhiteSpace(dto.UserName) ? dto.Email : dto.UserName;

            var user = new User
            {
                Email = dto.Email,
                UserName = userName,
                Username = userName,
                NormalizedEmail = dto.Email.ToUpperInvariant(),
                NormalizedUserName = userName.ToUpperInvariant(),
                IsVerified = true,
                EmailConfirmed = true
            };

            var res = await _userManager.CreateAsync(user, dto.Password);
            if (!res.Succeeded)
                return (false, string.Join("; ", res.Errors.Select(e => e.Description)));

            await SignInUserAsync(user);
            return (true, null);
        }

        private async Task SignInUserAsync(User user)
        {
            var displayName = string.IsNullOrWhiteSpace(user.UserName)
                              ? user.Email ?? $"user_{user.Id}"
                              : user.UserName;

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, displayName),
                new(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
                new("IsAdmin", user.IsAdmin.ToString())
            };

            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var principal = new ClaimsPrincipal(identity);

            var props = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            await _httpContext.HttpContext!.SignInAsync(
                IdentityConstants.ApplicationScheme,
                principal,
                props
            );
        }

        public async Task<UserPageDto?> GetAccountInfoAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                Console.WriteLine($"!!!🤬🤬🤬!! User with ID={id} not found");
                return null; // Повертаємо null якщо користувач не знайдений
            }

            return new UserPageDto
            {
                Username = user.Username,
                Email = user.Email
            };
            
        }
        
    }
 }
