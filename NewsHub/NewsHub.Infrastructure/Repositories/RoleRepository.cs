using NewsHub.Application.Interfaces.Roles;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories.ErrorCatch;
using NewsHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<RoleEnt>, IRoleRepository
    {
        public RoleRepository(
            ApplicationDbContext context, 
            ILogErrorRepository logError) : base(context, logError)
        {
        }

        public async Task<RoleEnt?> GetByNameAsync(string name)
        {
            return await _context.Roles.FindAsync(name);
        }
    }
}
