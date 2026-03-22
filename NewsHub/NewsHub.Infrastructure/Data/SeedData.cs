using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NewsHub.Infrastructure.Data;
using NewsHub.Domain.Entities;

namespace NewsHub.Infrastructure.Data
{
    public static class SeedData
    {
        private static readonly string[] DefaultRoles = new[] { "Admin", "Editor", "User" };

        // Call this at application startup to ensure the default roles exist in the application's own Role table.
        // Example (in Program.cs):
        // await SeedData.EnsureRolesAsync(app.Services);
        public static async Task EnsureRolesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            foreach (var roleName in DefaultRoles)
            {
                if (!db.Roles.Any(r => r.Name == roleName))
                {
                    db.Roles.Add(new RoleEnt(roleName));
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
