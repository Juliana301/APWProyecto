using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Interfaces.Services.Users;
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
                    IsActive = u.IsActive
                }).ToList();
            }

            return View(vm);
        }
    }
}
