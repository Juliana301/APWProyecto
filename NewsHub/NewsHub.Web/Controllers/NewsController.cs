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
                        id = item.UniqueId, // ID único basado en el SourceId y el hash de la URL

                        source = item.SourceName ?? "NOTFOUND", // Nombre de la fuente, o "NOTFOUND" si no se proporciona

                        type = item.SourceType.ToString(), // Tipo de fuente (RSS, API, etc.)

                        title = item.Title, // Título de la noticia

                        description = item.Description, // Descripción de la noticia

                        date = item.PublishedAt
                            .ToString("yyyy-MM-dd"),

                        tags = item.Category // Categorías o etiquetas asociadas a la noticia
                    });

            return Json(json);
        }
    }
}