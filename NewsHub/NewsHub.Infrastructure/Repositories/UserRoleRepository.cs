using NewsHub.Application.Interfaces.Roles;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;
using NewsHub.Domain.Interfaces.Repositories.ErrorCatch;
using NewsHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Repositories
{
    public class UserRoleRepository : GenericRepository<UserRoleEnt>, IUserRoleRepository
    {
        public UserRoleRepository(
            ApplicationDbContext context,
            ILogErrorRepository logError) : base(context, logError)
        {
        }


    }
}
