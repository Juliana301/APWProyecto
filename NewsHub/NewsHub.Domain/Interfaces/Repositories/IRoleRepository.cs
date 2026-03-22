using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Interfaces.Roles
{
    public interface IRoleRepository : IGenericRepository<RoleEnt>
    {
        Task<RoleEnt?> GetByNameAsync(string name);
    }
}
