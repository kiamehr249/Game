using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NikGame.Dart.Service;
using NikGame.Service;
using NikGame.Web.Models;
using System.Diagnostics;
using System.Linq;

namespace NikGame.Web.Controllers
{
    public class Home : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<Home> _logger;
        private readonly IDartService _iDartServ;

        public Home(
            IConfiguration config,
            ILogger<Home> logger, 
            IDartService iDartServ
            )
        {
            _config = config;
            _logger = logger;
            _iDartServ = iDartServ;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var matches = _iDartServ.iDartMatchServ.QueryMaker(y => y.Where(x => x.UserId != null && x.EndDate != null))
                .OrderByDescending(x => x.Id).Skip(0).Take(10).ToList();

            ViewBag.Matches = matches;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
