using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Constants;
using SimpleAuth.Data;
using SimpleAuth.Entity;

namespace SimpleAuth.Controllers;

[AllowAnonymous]
public class SeedingController : Controller
{
    private readonly ApplicationDbContext _context;

    public SeedingController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> SeedSuperAdmin()
    {
        try
        {
            var previousSuperAdminExists = await _context.Users.AnyAsync(x => x.UserType == UserTypeConstants.Admin);
            if (!previousSuperAdminExists)
            {
                using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                var admin = new User()
                {
                    Email = "super.admin",
                    UserType = UserTypeConstants.Admin,
                    Name = "Super Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin")
                };
                _context.Users.Add(admin);
                await _context.SaveChangesAsync();
                tx.Complete();
                return Content("User Seeding Complete");
            }

            return Content("User already seeded");
        }
        catch (Exception e)
        {
            return Content(e.Message);
        }
    }
}