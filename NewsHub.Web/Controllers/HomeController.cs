using Microsoft.AspNetCore.Mvc;
using NewsHub.Web.Models;
using System.Diagnostics;

namespace NewsHub.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
