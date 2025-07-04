using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.DTOs;
using MovieTheater.Infrastructure.Entities;
using System.Security.Claims;

[Route("")]
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> um, SignInManager<User> sm)
    {
        _userManager = um;
        _signInManager = sm;
    }

    // Pages (MVC)
    [HttpGet("Account/SignIn"), ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult SignInPage() => View("SignIn");

    [HttpGet("Account/SignUp"), ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult SignUpPage() => View("SignUp");

    // Forms (MVC)
    [ValidateAntiForgeryToken]
    [HttpPost("Account/Login"), ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> LoginMvc(LoginRequestDto dto)
    {
        if (!ModelState.IsValid) return View("SignIn");

        var res = await LoginInternalAsync(dto);
        if (!res.Success)
        {
            ModelState.AddModelError("", res.Error!);
            return View("SignIn");
        }
        return RedirectToAction("Index", "Home");
    }

    [ValidateAntiForgeryToken]
    [HttpPost("Account/Register"), ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> RegisterMvc(RegisterRequestDto dto)
    {
        if (!ModelState.IsValid) return View("SignUp");

        var res = await RegisterInternalAsync(dto);
        if (!res.Success)
        {
            ModelState.AddModelError("", res.Error!);
            return View("SignUp");
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpPost("Account/Logout"), ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> LogoutMvc()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return RedirectToAction("Index", "Home");
    }

    // API endpoints
    [HttpPost("api/account/login")]
    public async Task<IActionResult> LoginApi([FromBody] LoginRequestDto dto)
    {
        var res = await LoginInternalAsync(dto);
        return res.Success ? Ok() : Unauthorized(res.Error);
    }

    [HttpPost("api/account/register")]
    public async Task<IActionResult> RegisterApi([FromBody] RegisterRequestDto dto)
    {
        var res = await RegisterInternalAsync(dto);
        return res.Success ? Ok() : Conflict(res.Error);
    }

    [HttpPost("api/account/logout")]
    public async Task<IActionResult> LogoutApi()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }

    // Private methods
    private async Task<(bool Success, string? Error)> LoginInternalAsync(LoginRequestDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return (false, "bad creds");

        if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            return (false, "bad creds");

        await SignInUserAsync(user);
        return (true, null);
    }

    private async Task<(bool Success, string? Error)> RegisterInternalAsync(RegisterRequestDto dto)
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

        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, props);
    }
}
