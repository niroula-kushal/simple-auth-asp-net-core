using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Data;
using SimpleAuth.Manager;
using SimpleAuth.Manager.Interfaces;
using SimpleAuth.Provider;
using SimpleAuth.Provider.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(x => { x.LoginPath = "/Auth/Login"; });

builder.Services.AddControllers();
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<DbContext, ApplicationDbContext>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

var app = builder.Build();

app.Services.CreateScope().ServiceProvider.GetService<DbContext>().Database.Migrate();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();

app.Run();