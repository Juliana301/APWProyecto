using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Interfaces.Authentication;
using NewsHub.Application.Interfaces.Notifications;
using NewsHub.Application.Interfaces.Security;
using NewsHub.Application.Interfaces.Tokens;
using NewsHub.Application.Services.Authentication;
using NewsHub.Application.Services.Tokens;
using NewsHub.Domain.Interfaces.Repositories;
using NewsHub.Domain.Interfaces.Repositories.ErrorCatch;
using NewsHub.Domain.Interfaces.Repositories.Tokens;
using NewsHub.Infrastructure.Data;
using NewsHub.Infrastructure.Email.AzureEmail;
using NewsHub.Infrastructure.Repositories;
using NewsHub.Infrastructure.Repositories.CatchError;
using NewsHub.Infrastructure.Repositories.Tokens;
using NewsHub.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);


// ======================================================
// DATABASE
// ======================================================

var connectionString =
    Environment.GetEnvironmentVariable("NewsHub_DB_Connection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// ======================================================
// REPOSITORIES (Infrastructure Layer)
// ======================================================

builder.Services.AddScoped<ILogErrorRepository, LogErrorRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();



// ======================================================
// APPLICATION / SECURITY SERVICES
// ======================================================

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();

// ======================================================
// EMAIL SERVICE
// ======================================================

builder.Services.Configure<AzureEmailOptions>(
    builder.Configuration.GetSection("AzureEmail"));

builder.Services.AddScoped<IEmailSender, AzureEmailSender>();

// ======================================================
// AUTHENTICATION
// ======================================================

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.LogoutPath = "/Authentication/Logout";
        options.AccessDeniedPath = "/";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });


// ======================================================
// MVC + API
// ======================================================

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// ======================================================
// MIDDLEWARE PIPELINE
// ======================================================

// Swagger solo en Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // Importante
app.UseAuthorization();


// ======================================================
// ENDPOINTS
// ======================================================

app.MapControllers(); // API

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();