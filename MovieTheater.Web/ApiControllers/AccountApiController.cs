using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.DTOs;
using MovieTheater.Application.Interfaces;
using MovieTheater.Infrastructure.Entities;
using MovieTheater.Web.Infrastructure;

[Route("api/account")]
[ApiController]
public class AccountApiController : ControllerBase
{
        private readonly SignInManager<User> _signInManager;
        private readonly IAccountService _accountService;

        public AccountApiController(
            SignInManager<User> signInManager,
            IAccountService accountService)
        {
            _signInManager = signInManager;
            _accountService = accountService;
        }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (!ModelState.IsValid)
            return ApiProblem.Bad("Bad request", "Email & Password required");

        var (ok, err) = await _accountService.LoginAsync(dto);
        if (!ok) return ApiProblem.Unauthorized("Invalid credentials", err!);

        await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, false);
        return Ok(new { message = "login ok" });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        if (!ModelState.IsValid)
            return ApiProblem.Bad("Bad request", "Email / UserName / Password required");

        var (ok, err) = await _accountService.RegisterAsync(dto);
        if (!ok)
        {
            if (err == "email exists")   return ApiProblem.Conflict("Email exists", err);
            return ApiProblem.Bad("Validation error", err!);
        }

        await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, false);
        return Ok(new { message = "signup ok" });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }
}
