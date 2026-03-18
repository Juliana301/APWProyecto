using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Interfaces.Roles
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role?> GetByNameAsync(string name);
    }
}
