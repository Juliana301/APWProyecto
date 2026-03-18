using NewsHub.Application.Interfaces.Roles;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories.ErrorCatch;
using NewsHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(
            ApplicationDbContext context, 
            ILogErrorRepository logError) : base(context, logError)
        {
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Roles.FindAsync(name);
        }
    }
}
