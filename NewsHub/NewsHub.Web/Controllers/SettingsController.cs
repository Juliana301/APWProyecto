using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsHub.Application.DTOs.Secret;
using NewsHub.Application.DTOs.User;
using NewsHub.Application.Interfaces.Services.Secret;
using NewsHub.Application.Interfaces.Services.Source;
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
        private readonly ISecretService _secretService;
        private readonly ISourceService _sourceService;

        public SettingsController(
            IManageUsersService manageUsersService,
            ISecretService secretService,
            ISourceService sourceService)
        {
            _manageUsersService = manageUsersService;
            _secretService = secretService;
            _sourceService = sourceService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await BuildSettingsViewModelAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UpdateUserViewModel vm)
        {
            var dto = new UpdateUserSettingsDto
            {
                UserId = vm.UserId,
                Role = vm.Role,
                IsActive = vm.IsActive
            };

            var result = await _manageUsersService.UpdateUserSettingsAsync(dto);

            if (!result.Success)
            {
                TempData.SetToast(result.Error ?? "Error al actualizar el usuario", result.TypeMessage);
                return RedirectToAction(nameof(Index));
            }

            TempData.SetToast(result.Message ?? "Usuario actualizado correctamente", result.TypeMessage);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSecret(
        [Bind(Prefix = nameof(SettingsViewModel.NewSecret))] CreateSecretViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var pageVm = await BuildSettingsViewModelAsync();
                pageVm.NewSecret = vm;
                return View(nameof(Index), pageVm);
            }

            var dto = new CreateSecretDto
            {
                Key = vm.Key,
                Value = vm.Value,
                IsEncrypted = vm.IsEncrypted,
                SourceId = vm.SourceId
            };

            var result = await _secretService.CreateAsync(dto);

            if (!result.Success)
            {
                TempData.SetToast(result.Error ?? "Error al agregar el secret", result.TypeMessage);
                return RedirectToAction(nameof(Index));
            }

            TempData.SetToast(result.Message ?? "Secret agregado correctamente", result.TypeMessage);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSecret(DeleteSecretViewModel vm)
        {
            var result = await _secretService.DeleteAsync(vm.Id);

            if (!result.Success)
            {
                TempData.SetToast(result.Error ?? "Error al eliminar el secret", result.TypeMessage);
                return RedirectToAction(nameof(Index));
            }

            TempData.SetToast(result.Message ?? "Secret eliminado correctamente", result.TypeMessage);
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

        private async Task<SettingsViewModel> BuildSettingsViewModelAsync()
        {
            var users = await _manageUsersService.GetAllUsersAsync(includeInactive: true);
            var secrets = await _secretService.GetAllAsync();
            var sources = await _sourceService.GetAllAsync();

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

            if (secrets.Success && secrets.Data != null)
            {
                vm.Secrets = secrets.Data.Select(s => new SecretViewModel
                {
                    Id = s.Id,
                    Key = s.Key,
                    Value = s.Value,
                    IsEncrypted = s.IsEncrypted,
                    SourceName = s.Source.Name
                }).ToList();
            }

            if (sources.Success && sources.Data != null)
            {
                vm.SourceItems = sources.Data.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList();
            }

            return vm;
        }
    }
}