using FamousQuoteQuiz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FamousQuoteQuiz.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Authorize(Roles ="Admin,User")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin,User")]
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize(Roles = "User")]
        public IActionResult About()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
