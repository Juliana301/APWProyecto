using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.DTOs.Authentication;
using NewsHub.Application.DTOs.User;
using NewsHub.Application.Interfaces.Authentication;
using NewsHub.Application.Interfaces.Tokens;
using NewsHub.Web.Common.Extensions;
using NewsHub.Web.ViewModels.Authentication;
using System.Security.Claims;

namespace NewsHub.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IPasswordResetTokenService _passwordResetTokenService;

        public AuthenticationController(
            IAuthService authService,
            IPasswordResetTokenService passwordResetTokenService
            )
        {
            _authService = authService;
            _passwordResetTokenService = passwordResetTokenService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new LoginDto
            {
                UserName = model.UserName,
                Password = model.Password
            };

            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error!);
                return View(model);
            }

            var user = result.Data!;
            var claims = GetClaims(user);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(50)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return RedirectToAction("Index","News");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) 
            {
                return View(model);
            }

            var dto = new RegisterDto {
                Email = model.Email,
                UserName = model.UserName,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _authService.RegisterAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error!);
                return View(model);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        [Authorize]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new ForgotPasswordDto { Email = model.Email };

            var result = await _authService.ForgotPasswordAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error!);
                return View(model);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login");

            var result = await _passwordResetTokenService.ValidateTokenAsync(token);

            if (!result.Success)
            {
                TempData.SetToast(result.Error!, result.TypeMessage);
                return RedirectToAction("Login");
            }

            var model = new PasswordResetViewModel
            {
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(PasswordResetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction("Login");
        }


        public static List<Claim> GetClaims(UserDto dto)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, dto.FirstName!),
                new Claim(ClaimTypes.Email, dto.Email),
                new Claim(ClaimTypes.NameIdentifier, dto.Id.ToString()),
            };

            foreach (var role in dto.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
