using Microsoft.AspNetCore.Authentication.JwtBearer;
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

namespace NikGame.Web.Apis
{
    [Route("/api/[controller]/[action]")]
    public class DartsPlay : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly IDartService _iDartServ;
        private readonly IHubContext<DartHub> _pushService;

        public DartsPlay(
            IConfiguration config,
            UserManager<User> userManager,
            IDartService iDartServ,
            IHubContext<DartHub> pushService
            )
        {
            _config = config;
            _userManager = userManager;
            _iDartServ = iDartServ;
            _pushService = pushService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> StartGame()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            DartMatch match = null;

            var currentMatch = _iDartServ.iDartMatchServ.Find(x => x.EndDate == null);
            if (currentMatch != null && currentMatch.StartDate.AddMinutes(5) > DateTime.Now)
            {
                currentMatch.EndDate = currentMatch.StartDate.AddMinutes(5);
                await _iDartServ.iDartMatchServ.SaveChangesAsync();

                //Do push signal to update top winner list
            }
            else if(currentMatch != null)
            {
                match = currentMatch;
            }

            if (match == null)
            {
                match = new DartMatch();
                match.Title = "Dart Game";
                _iDartServ.iDartMatchServ.Add(match);
                await _iDartServ.iDartMatchServ.SaveChangesAsync();
            }

            match.Players++;

            if (match.Players > 1)
            {
                match.StartDate = DateTime.Now;
                //Do Push allow start game
                _pushService.Clients.All.SendAsync("AllowStartGame", match.Id);
            }
            

            var matchUser = new DartMatchUser
            {
                UserId = user.Id,
                MatchId = match.Id
            };

            _iDartServ.iDartMatchUserServ.Add(matchUser);
            await _iDartServ.iDartMatchUserServ.SaveChangesAsync();

            var opponents = _iDartServ.iDartMatchUserServ.QueryMaker(y => y.Where(x => x.MatchId == match.Id)).Select(x => new
            {
                x.Id,
                x.UserId,
                x.User.FirstName,
                x.User.LastName
            }).ToList();

            return Ok(new
            {
                title = "New Match",
                status = 200,
                data = new
                {
                    mathch = new
                    {
                        match.Id,
                        match.Title,
                        match.StartDate
                    },
                    opponents,
                    startState = match.Players > 1 ? true : false,
                }
            });


        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DartsShoot(int matchId, int score)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //var match = await _iDartServ.iDartMatchServ.FindAsync(x => x.Id == matchId);
            //if(match == null)
            //{
            //    return BadRequest(new {
            //        title = "Darts Shoot",
            //        message = "The parameters was not valid",
            //        data = ""
            //    });
            //}

            var mUser = _iDartServ.iDartMatchUserServ.Find(x => x.UserId == user.Id && x.MatchId == matchId);
            if (mUser == null)
            {
                return BadRequest(new
                {
                    title = "Darts Shoot",
                    message = "The parameters was not valid",
                    data = ""
                });
            }

            mUser.TotalScore += score;

            var shoot = new DartShoot
            {
                MatchId = matchId,
                UserId = user.Id,
                MatchUserId = mUser.Id,
                ShotScore = score,
                CreateDate = DateTime.Now
            };

            _iDartServ.iDartShootServ.Add(shoot);
            await _iDartServ.iDartShootServ.SaveChangesAsync();

            //Do push update score board
            _pushService.Clients.All.SendAsync("UpdateScoreList", matchId);

            return Ok(new { 
                title = "Darts Shoot",
                message = "Success Saved",
                data = new
                {
                    shoot = new {
                        shoot.Id,
                        shoot.ShotScore,
                        shoot.CreateDate
                    },
                    mUser.TotalScore
                }
            });
        }

        [HttpGet]
        public IActionResult GetTopWinners(int size)
        {
            var winers = _iDartServ.iDartMatchServ.QueryMaker(y => y.Where(x => x.EndDate != null)).Select(x => new
            {
                x.Id,
                x.Title,
                x.Winer.FirstName,
                x.Winer.LastName,
                x.WinerScore
            }).OrderByDescending(x => x.WinerScore).Skip(0).Take(size).ToList();

            return Ok(new
            {
                title = "Winner List",
                status = 200,
                data = winers
            });
        }

        [HttpGet]
        public IActionResult GetTopMatchUsers(int matchId, int size)
        {
            var top3 = _iDartServ.iDartMatchUserServ.QueryMaker(y => y.Where(x => x.MatchId == matchId)).Select(x => new
            {
                x.Id,
                x.MatchId,
                x.UserId,
                x.User.FirstName,
                x.User.LastName,
                x.TotalScore,
                x.DartMatch.Title
            }).OrderByDescending(x => x.TotalScore).Skip(0).Take(size).ToList();

            return Ok(new
            {
                title = "Top Match Player",
                status = 200,
                data = top3
            });
        }

        [HttpGet]
        public async Task<IActionResult> TestPush()
        {
            await _pushService.Clients.All.SendAsync("ReceiveMessage", "Kiamehr", "Hey There");
            return Ok();
        }


    }
}
