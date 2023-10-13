using Frilou_UI_V2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Frilou_UI_V2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult HomePage()
        {
            return View();
        }

        public IActionResult Account()
        {
            return View();
        }

        public IActionResult BillOfMaterials()
        {
            return View();  
        }

        public IActionResult GenerateBOM()
        {
            return View();  
        }

        public IActionResult MaterialCostEstimate()
        {
            return View();
        }

        public IActionResult AddProduct()
        {
            return View();
        }


        public IActionResult EditProduct()
        {
            return View();
        }

        public IActionResult AddEmployee()
        {
            return View();
        }

        public IActionResult EditEmployee()
        {
            return View();
        }

        public IActionResult AddPartner()
        {
            return View();
        }

        public IActionResult EditPartner()
        {
            return View();
        }

        public IActionResult LogIn()
        {
            return View();
        }

        public IActionResult AccountPartner()
        {
            return View();
        }

        public IActionResult AccountEmployee()
        {
            return View();
        }

        public IActionResult Formula()
        {
            return View();
        }

        public IActionResult Preset()
        {
            return View();
        }

        public IActionResult AddPreset()
        {
            return View();
        }

        public IActionResult EditPreset()
        {
            return View();
        }

        public IActionResult AddClient()
        {
            return View();
        }

        public IActionResult EditClient()
        {
            return View();
        }

        public IActionResult AddProject()
        {
            return View();
        }

        public IActionResult EditProject()
        {
            return View();
        }
        public IActionResult HomepageAdmin()
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