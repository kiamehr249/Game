using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using NikGame.Dart.Service;
using NikGame.Web.PushService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NikGame.Web.Controllers
{
    public class Test : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<DartUserHub> _iDartUserHubContext;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IConfiguration _config;
        private readonly IDartService _iDartServ;
        public List<string> Messages;

        public Test(
            IConfiguration config,
            UserManager<User> userManager,
            IDartService iDartServ,
            IHubContext<DartUserHub> iDartUserHubContext,
            IUserConnectionManager userConnectionManager
            )
        {
            _config = config;
            _userManager = userManager;
            _iDartServ = iDartServ;
            Messages = new List<string>();
            _iDartUserHubContext = iDartUserHubContext;
            _userConnectionManager = userConnectionManager;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _iDartServ.iUserServ.QueryMaker(y => y.Where(x => true)).Select(x => new
            {
                x.Id,
                x.FirstName,
                x.LastName
            }).OrderBy(x => x.Id)
                .Skip(0).Take(10).ToList();
            return Ok(new
            {
                status = 200,
                data = users
            });
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Messaging()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = user;
            return View();
        }

    }
}
