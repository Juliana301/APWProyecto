using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Configuration;
using NewsHub.Application.External;
using NewsHub.Application.Interfaces;
using NewsHub.Application.Interfaces.Authentication;
using NewsHub.Application.Interfaces.Notifications;
using NewsHub.Application.Interfaces.Persistence;
using NewsHub.Application.Interfaces.Roles;
using NewsHub.Application.Interfaces.Security;
using NewsHub.Application.Interfaces.Services;
using NewsHub.Application.Interfaces.Tokens;
using NewsHub.Application.Services.Authentication;
using NewsHub.Application.Services.Roles;
using NewsHub.Application.Services.Source;
using NewsHub.Application.Services.Tokens;
using NewsHub.Domain.Interfaces.Repositories;
using NewsHub.Domain.Interfaces.Repositories.ErrorCatch;
using NewsHub.Domain.Interfaces.Repositories.Tokens;
using NewsHub.Infrastructure.Data;
using NewsHub.Infrastructure.Email.AzureEmail;
using NewsHub.Infrastructure.External;
using NewsHub.Infrastructure.Repositories;
using NewsHub.Infrastructure.Repositories.CatchError;
using NewsHub.Infrastructure.Repositories.Tokens;
using NewsHub.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);


// ======================================================
// LOGGING
// ======================================================

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();


// ======================================================
// DATABASE (Infrastructure)
// ======================================================

DotNetEnv.Env.Load();

builder.Configuration
    .AddEnvironmentVariables();

var connectionString =
    Environment.GetEnvironmentVariable("NewsHub_DB_Connection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Database connection string is not configured.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


// ======================================================
// CONFIGURATIONS (Application Settings)
// ======================================================

var baseUrl = builder.Configuration["AppSettings:BaseUrl"];

if (string.IsNullOrWhiteSpace(baseUrl))
    throw new InvalidOperationException("AppSettings:BaseUrl is not configured.");

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

builder.Services.Configure<AzureEmailOptions>(
    builder.Configuration.GetSection("AzureEmail"));


// ======================================================
// REPOSITORIES (Infrastructure Layer)
// ======================================================

builder.Services.AddScoped<ILogErrorRepository, LogErrorRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ISourceRepository, SourceRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();


// ======================================================
// APPLICATION SERVICES (Business Logic)
// ======================================================

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<ISourceService, SourceService>();
builder.Services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();


// ======================================================
// PARSERS
// ======================================================

builder.Services.AddHttpClient();

builder.Services.AddScoped<ISourceReader, RssSourceReader>();

builder.Services.AddScoped<SourceReaderFactory>();

// ======================================================
// SECURITY SERVICES
// ======================================================

builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();


// ======================================================
// EXTERNAL SERVICES (Email, etc.)
// ======================================================

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
// SEED DATA
// ======================================================

using (var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider;

    // var db = provider.GetRequiredService<ApplicationDbContext>();
    // db.Database.Migrate();

    await SeedData.EnsureRolesAsync(provider);
}


// ======================================================
// GLOBAL EXCEPTION HANDLING
// ======================================================

var logger = app.Services.GetRequiredService<ILogger<Program>>();

AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
{
    var ex = eventArgs.ExceptionObject as Exception;
    logger.LogCritical(ex, "Unhandled exception occurred.");
};

TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
{
    logger.LogCritical(eventArgs.Exception, "Unobserved task exception.");
    eventArgs.SetObserved();
};


// ======================================================
// MIDDLEWARE PIPELINE (Web Layer)
// ======================================================

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


// ======================================================
// ENDPOINTS
// ======================================================

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();