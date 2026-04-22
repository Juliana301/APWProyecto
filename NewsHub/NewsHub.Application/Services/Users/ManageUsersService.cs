using NewsHub.Application.Common;
using NewsHub.Application.DTOs.User;
using NewsHub.Application.Interfaces.Services.Users;
using NewsHub.Domain.Enums;
using NewsHub.Domain.Interfaces.Repositories;
using NewsHub.Application.Interfaces.Persistence;
using NewsHub.Application.Interfaces.Roles;

namespace NewsHub.Application.Services.Users
{
    public class ManageUsersService : IManageUsersService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ManageUsersService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<UserDto>>> GetAllUsersAsync(bool includeInactive)
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync(includeInactive);

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

        public async Task<Result<bool>> UpdateUserSettingsAsync(UpdateUserSettingsDto dto)
        {
            try
            {
                var user = await _userRepository.GetByIdWithRolesAsync(dto.UserId);

                if (user == null)
                    return Result<bool>.Fail("Usuario no encontrado.", TypeMessage.Warning);

                var currentRoleName = user.UserRoles.FirstOrDefault()?.Role.Name;
                var isCurrentlyAdmin = string.Equals(currentRoleName, "Admin", StringComparison.OrdinalIgnoreCase);

                var isChangingFromAdminToNonAdmin = isCurrentlyAdmin && dto.Role != RolesEnums.Admin;
                var isDeactivatingAdmin = isCurrentlyAdmin && !dto.IsActive;

                if (isChangingFromAdminToNonAdmin || isDeactivatingAdmin)
                {
                    var activeAdmins = await _userRepository.CountActiveAdminsAsync();

                    if (activeAdmins <= 1)
                    {
                        return Result<bool>.Fail(
                            "Debe existir al menos un administrador activo en el sistema.",
                            TypeMessage.Warning);
                    }
                }

                var role = await _roleRepository.FirstAsync(r => r.Name == dto.Role.ToString());

                if (role == null)
                    return Result<bool>.Fail("Rol no encontrado.", TypeMessage.Warning);

                user.UpdateRole(role.Id);

                if (dto.IsActive)
                    user.Activate();
                else
                    user.Deactivate();

                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error actualizando usuario: {ex.Message}", TypeMessage.Error);
            }
        }
    }
}