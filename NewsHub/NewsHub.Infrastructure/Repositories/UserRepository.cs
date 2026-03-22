using Microsoft.EntityFrameworkCore;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;
using NewsHub.Domain.Interfaces.Repositories.ErrorCatch;
using NewsHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<UserEnt>, IUserRepository
    {

        public UserRepository(
            ApplicationDbContext context,
            ILogErrorRepository logError) : base(context, logError)
        {
        }

        public async Task<UserEnt?> GetByEmailAsync(string email)
        {
            return await FirstAsync(u => u.Email == email);
        }

        public async Task<UserEnt?> GetByUserNameAsync(string userName)
        {
            return await FirstAsync(
                u => u.UserName == userName,
                include: query => query
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role));
        }
    }
}
