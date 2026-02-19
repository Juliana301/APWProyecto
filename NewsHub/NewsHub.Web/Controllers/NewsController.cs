using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NewsHub.Web.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        [HttpGet]
        public IActionResult Index(string search)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
