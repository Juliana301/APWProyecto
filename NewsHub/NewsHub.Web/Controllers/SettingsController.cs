using Microsoft.AspNetCore.Mvc;

namespace NewsHub.Web.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
