using NewsHub.Application.Common;
using NewsHub.Application.Interfaces.Persistence;
using NewsHub.Application.Interfaces.Roles;
using NewsHub.Application.Interfaces.Services.Roles;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Interfaces.Repositories;

namespace NewsHub.Application.Services.Roles
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserRoleService(
            IUserRoleRepository userRoleRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> AddUserRoleAsync(int userId, string[] roles)
        {
            if (roles == null || roles.Length == 0)
                return Result<bool>.Fail("Debe especificar al menos un rol.", TypeMessage.Warning);

            var roleNames = roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct()
                .ToList();

            var roleEntities = await _roleRepository.FindAsync(r => roleNames.Contains(r.Name));

            if (!roleEntities.Any())
                return Result<bool>.Fail("Ninguno de los roles enviados es válido.", TypeMessage.Warning);

            var foundRoleNames = roleEntities.Select(r => r.Name).ToHashSet();

            var missingRoles = roleNames
                .Where(r => !foundRoleNames.Contains(r))
                .ToList();

            if (missingRoles.Any())
                return Result<bool>.Fail(
                    $"Roles no encontrados: {string.Join(", ", missingRoles)}",
                    TypeMessage.Warning
                );

            var roleIds = roleEntities.Select(r => r.Id).ToList();

            var existingUserRoles = await _userRoleRepository.FindAsync(
                ur => ur.UserId == userId && roleIds.Contains(ur.RoleId)
            );

            var existingRoleIds = existingUserRoles
                .Select(ur => ur.RoleId)
                .ToHashSet();

            var newUserRoles = roleIds
                .Where(roleId => !existingRoleIds.Contains(roleId))
                .Select(roleId => new UserRoleEnt(userId, roleId))
                .ToList();

            foreach (var userRole in newUserRoles)
            {
                await _userRoleRepository.AddAsync(userRole);
            }

            return Result<bool>.Ok(true, "Roles asignados correctamente.");
        }
    }
}