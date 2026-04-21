using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Interfaces.Services.Users;

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
        public IActionResult Index()
        {

            return View();
        }

        // WORK IN PROGRESS
        private IActionResult GetUsersListView()
        {
            var users = _manageUsersService.GetAllUsersAsync(includeInactive: true).Result;
            return PartialView("_UsersListPartial", users);
        }
    }
}
