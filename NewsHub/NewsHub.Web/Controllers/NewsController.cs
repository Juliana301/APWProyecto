using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Interfaces.Services.Source;
using NewsHub.Web.ViewModels.Dashboard;
using NewsHub.Web.ViewModels.Source;

namespace NewsHub.Web.Controllers
{
    public class NewsController : Controller
    {
        private readonly ISourceService _sourceService;

        public NewsController(
            ISourceService sourceService
            )
        {
            _sourceService = sourceService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var result = await _sourceService.GetAllAsync();

            var vm = new DashboardViewModel();

            if (result.Success && result.Data != null)
            {
                vm.Sources = result.Data.Select(s => new ListSourceViewViewModel
                {
                    Id = s.Id,
                    Url = s.Url,
                    Name = s.Name,
                    Description = s.Description,
                    ComponentType = s.ComponentType,
                    RequiresSecret = s.RequiresSecret,
                    ApiConfigJson = s.ApiConfigJson
                }).ToList();
            }

            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}