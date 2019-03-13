#pragma warning disable 1591
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WM.GUID.WebAPI.Models;

namespace WM.GUID.WebAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Swagger()
        {
            return Redirect("/swagger");
        }

        public IActionResult HealthChecks()
        {
            return Redirect("/healthchecks-ui");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
