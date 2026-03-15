using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NewsHub.Web.Controllers
{
    public class NewsController : Controller
    {
        [HttpGet]
        [Authorize]
        public IActionResult Index(string search)
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
