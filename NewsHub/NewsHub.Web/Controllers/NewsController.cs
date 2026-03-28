using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Interfaces.Services;
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
        public async Task<IActionResult> Index(string search)
        {
            var result = await _sourceService.GetAllAsync();

            var vm = new DashboardViewModel();

            if (result.Success && result.Data != null)
            {
                vm.Sources = result.Data.Select(s => new SourceViewModel
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFeedJson()
        {
            var result =
                await _sourceService
                    .ReadAllFeedsAsync();

            if (!result.Success || result.Data == null)
                return Json(new List<object>());

            var json =
                result.Data
                    .OrderByDescending(x => x.PublishedAt)
                    .Select((item, index) => new
                    {
                        id = index + 1,

                        source = item.SourceName ?? "RSS",

                        type = "feed",

                        title = item.Title,

                        description = item.Description,

                        date = item.PublishedAt
                            .ToString("yyyy-MM-dd"),

                        tags = item.Category
                    });

            return Json(json);
        }
    }
}