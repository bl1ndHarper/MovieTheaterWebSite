using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.DTOs;
using MovieTheater.Application.Interfaces;
using MovieTheater.Infrastructure.Entities;
using MovieTheater.Web.ViewModels;
using System.Security.Claims;

[Route("")]
public class AccountController : Controller
{
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAccountService _accountService;
        private readonly IBookingService _bookingService;

        public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IAccountService accountService,
        IBookingService bookingService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService;
            _bookingService = bookingService;
        }

    // Pages (MVC)
    [HttpGet("Account/SignIn"), ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult SignInPage() => View("SignIn");

    [HttpGet("Account/SignUp"), ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult SignUpPage() => View("SignUp");

    [HttpGet("Account/ProfilePage/{accountId}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Account(long accountId)
    {
        
        var userDto = await _accountService.GetAccountInfoAsync(accountId);
        if (userDto == null)
        {
            return NotFound();
        }

        var bookings = await _bookingService.GetGroupedBookingsAsync(accountId);

        var viewModel = new AccountPageViewModel
        {
            Username = userDto.Username,
            Email = userDto.Email,
            Bookings = bookings
        };
        

        return View(viewModel);
    }

    // Forms (MVC)
    [ValidateAntiForgeryToken]
    [HttpPost("Account/Login"), ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> LoginMvc(LoginRequestDto dto)
    {
        if (!ModelState.IsValid) return View("SignIn");

        var res = await _accountService.LoginAsync(dto);
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

        var res = await _accountService.RegisterAsync(dto);
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
}