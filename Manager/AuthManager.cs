using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Data;
using SimpleAuth.Manager.Interfaces;

namespace SimpleAuth.Manager;

public class AuthManager : IAuthManager
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthManager(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Login(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == username.ToLower().Trim());
        if (user == null)
        {
            throw new Exception("Invalid username");
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new Exception("Username and password do not match");
        }

        var httpContext = _httpContextAccessor.HttpContext;
        var claims = new List<Claim>
        {
            new("Id", user.Id.ToString())
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));
    }

    public async Task Logout()
    {
        await _httpContextAccessor.HttpContext.SignOutAsync();
    }
}