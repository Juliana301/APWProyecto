using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.DTOs.User;
using NewsHub.Application.Interfaces.Services.Users;
using NewsHub.Domain.Enums;
using NewsHub.Web.Common.Extensions;
using NewsHub.Web.ViewModels.Settings;

namespace NewsHub.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly IManageUsersService _manageUsersService;

        public SettingsController(
            IManageUsersService manageUsersService)
        {
            _manageUsersService = manageUsersService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _manageUsersService.GetAllUsersAsync(includeInactive: true);

            var vm = new SettingsViewModel();

            if (users.Success && users.Data != null)
            {
                vm.Users = users.Data.Select(u => new UsersViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    IsActive = u.IsActive,
                    Role = MapRole(u.Roles.FirstOrDefault())
                }).ToList();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UpdateUserSettingsDto dto)
        {
            var result = await _manageUsersService.UpdateUserSettingsAsync(dto);

            if (!result.Success)
            {
                TempData.SetToast(result.Error ?? "Error al actualizar el usuario", result.TypeMessage);
                return RedirectToAction(nameof(Index));
            }

            TempData.SetToast(result.Message ?? "Usuario actualizado correctamente", result.TypeMessage);
            return RedirectToAction(nameof(Index));
        }

        private static RolesEnums MapRole(string? roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return RolesEnums.User;

            var normalized = roleName.Trim();

            if (Enum.TryParse<RolesEnums>(normalized, true, out var parsed))
                return parsed;

            foreach (var role in Enum.GetValues<RolesEnums>())
            {
                if (string.Equals(EnumExtensions.GetDescription(role), normalized, StringComparison.OrdinalIgnoreCase))
                    return role;
            }

            return RolesEnums.User;
        }
    }
}
