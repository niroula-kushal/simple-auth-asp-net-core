using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAuth.Data;
using SimpleAuth.Manager.Interfaces;
using SimpleAuth.ViewModels.Auth;

namespace SimpleAuth.Controllers;

[AllowAnonymous]
public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthManager _authManager;

    public AuthController(ApplicationDbContext context, IAuthManager authManager)
    {
        _context = context;
        _authManager = authManager;
    }

    public IActionResult Login()
    {
        var vm = new LoginVm();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        try
        {
            await _authManager.Login(vm.Username, vm.Password);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            vm.ErrorMessage = e.Message;
            return View(vm);
        }
    }

    public async Task<IActionResult> Logout()
    {
        await _authManager.Logout();
        return RedirectToAction("Login");
    }
}