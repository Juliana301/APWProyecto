using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsHub.Services.Services;
using System.Text.Json;

namespace NewsHub.Web.Controllers
{
    public class NewsController : Controller
    {
        private readonly SourceItemService _sourceItemService;

        public NewsController(SourceItemService sourceItemService)
        {
            _sourceItemService = sourceItemService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search)
        {
            var items = await _sourceItemService.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(item =>
                {
                    var doc = JsonDocument.Parse(item.Json);

                    if (!doc.RootElement.TryGetProperty("title", out var titleElement))
                        return false;

                    var title = titleElement.GetString();

                    return title != null &&
                           title.Contains(search, StringComparison.OrdinalIgnoreCase);
                }).ToList();
            }

            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var news = await _sourceItemService.GetByIdAsync(id);

            if (news == null)
                return NotFound();

            return View(news);
        }
    }
}