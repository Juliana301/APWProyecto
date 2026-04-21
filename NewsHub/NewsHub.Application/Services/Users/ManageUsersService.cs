using NewsHub.Application.Common;
using NewsHub.Application.DTOs.User;
using NewsHub.Application.Interfaces.Services.Users;
using NewsHub.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Services.Users
{
    public class ManageUsersService : IManageUsersService
    {
        private readonly IUserRepository _userRepository;

        public ManageUsersService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<List<UserDto>>> GetAllUsersAsync(bool includeInactive)
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync(includeInactive);

                // Map UserEnt to UserDto
                var userDtos = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    IsActive = u.IsActive,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                }).ToList();

                return Result<List<UserDto>>.Ok(userDtos);
            }
            catch (Exception ex)
            {
                return Result<List<UserDto>>.Fail($"Error obteniendo usuarios: {ex.Message}", TypeMessage.Error);
            }
        }
    }
}