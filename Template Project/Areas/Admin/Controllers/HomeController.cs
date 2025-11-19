using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Template_Project.Models;
using Template_Project.Utilities;

namespace Template_Project.Controllers
{
    [Area("Admin")]
<<<<<<< HEAD
    [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
=======
>>>>>>> 47837ba82ed9ef513408b815d13bf8256bfb6f04
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize(Roles = $"{SData.SUPPER_ADMIN_ROLE}, {SData.ADMIN_ROLE}, {SData.EMPLOYEE_ROLE}")]
        public IActionResult Index()
        {
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
