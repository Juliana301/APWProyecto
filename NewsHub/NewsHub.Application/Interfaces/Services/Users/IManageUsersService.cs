using NewsHub.Application.Common;
using NewsHub.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Interfaces.Services.Users
{
    public interface IManageUsersService
    {
        Task<Result<List<UserDto>>> GetAllUsersAsync(bool includeInactive);
    }
}
