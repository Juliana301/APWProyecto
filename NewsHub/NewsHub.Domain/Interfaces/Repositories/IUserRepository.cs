using NewsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<UserEnt>
    {
        Task<UserEnt?> GetByEmailAsync(string email);
        Task<UserEnt?> GetByUserNameAsync(string userName);
        Task<List<UserEnt>> GetAllUsersAsync(bool includeInactive);
        Task<UserEnt?> GetByIdWithRolesAsync(int id);
    }  
}
