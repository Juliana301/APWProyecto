using NewsHub.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Interfaces.Services.Users
{
    public interface IManageUsersService
    {
        Task<List<UserDto>> GetAllUsersAsync(bool includeInactive);
    }
}
