using NewsHub.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Interfaces.Roles
{
    public interface IUserRoleService
    {
        Task<Result<bool>> AddUserRoleAsync(int userId, string[] roles);
    }
}
