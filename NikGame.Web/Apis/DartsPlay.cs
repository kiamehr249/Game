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

            if (currentMatch != null)
            {
                var endDate = currentMatch.StartDate.AddMinutes(5);
                if (endDate < DateTime.Now)
                {
                    var winer = _iDartServ.iDartMatchUserServ.QueryMaker(y => y.Where(x => x.MatchId == currentMatch.Id)).OrderByDescending(x => x.TotalScore).Skip(0).Take(1).ToList();
                    currentMatch.EndDate = currentMatch.StartDate.AddMinutes(5);
                    if (winer.Count > 0)
                    {
                        currentMatch.UserId = winer[0].UserId;
                        currentMatch.WinerScore = winer[0].TotalScore;
                    }
                    await _iDartServ.iDartMatchServ.SaveChangesAsync();
                }
                else
                {
                    match = currentMatch;
                }
            }

            if (match == null)
            {
                match = new DartMatch();
                match.Title = "Dart Game";
                match.StartDate = DateTime.Now;
                match.Players = 1;
                _iDartServ.iDartMatchServ.Add(match);
                await _iDartServ.iDartMatchServ.SaveChangesAsync();
            }
            else
            {
                match.Players++;
            }

            var matchUser = await _iDartServ.iDartMatchUserServ.FindAsync(x => x.UserId == user.Id && x.MatchId == match.Id);
            if (matchUser == null)
            {
                matchUser = new DartMatchUser
                {
                    UserId = user.Id,
                    MatchId = match.Id
                };
                _iDartServ.iDartMatchUserServ.Add(matchUser);
                await _iDartServ.iDartMatchUserServ.SaveChangesAsync();
            }


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
                    match = new
                    {
                        match.Id,
                        match.Title,
                        match.StartDate
                    },
                    opponents,
                    totalScore = matchUser.TotalScore
                }
            });


        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DartsShoot(int matchId, int score)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var match = await _iDartServ.iDartMatchServ.FindAsync(x => x.Id == matchId);
            if (match == null)
            {
                return BadRequest(new
                {
                    title = "Darts Shoot",
                    status = 500,
                    message = "The parameters was not valid",
                    gameOver = false,
                    data = ""
                });
            }

            if (match.StartDate.AddMinutes(5) < DateTime.Now)
            {
                match.EndDate = match.StartDate.AddMinutes(5);
                var winer = _iDartServ.iDartMatchUserServ.QueryMaker(y => y.Where(x => x.MatchId == matchId)).OrderByDescending(x => x.TotalScore).Skip(0).Take(1).ToList();
                if(winer.Count > 0)
                {
                    match.UserId = winer[0].UserId;
                    match.WinerScore = winer[0].TotalScore;
                }
                
                await _iDartServ.iDartMatchServ.SaveChangesAsync();
                return BadRequest(new
                {
                    title = "Darts Shoot",
                    status = 401,
                    message = "The game is over",
                    gameOver = true,
                    data = "",
                });
            }

            var mUser = _iDartServ.iDartMatchUserServ.Find(x => x.UserId == user.Id && x.MatchId == matchId);
            if (mUser == null)
            {
                return BadRequest(new
                {
                    title = "Darts Shoot",
                    status = 400,
                    message = "The parameters was not valid",
                    gameOver = false,
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
            if (score > 0)
            {
                _pushService.Clients.All.SendAsync("UpdateMatchTops", matchId);
            }

            return Ok(new
            {
                title = "Darts Shoot",
                status = 200,
                message = "Success Saved",
                gameOver = false,
                data = new
                {
                    shoot = new
                    {
                        shoot.Id,
                        shoot.ShotScore,
                        shoot.CreateDate
                    },
                    mUser.TotalScore
                }
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> FinishGame(int matchId)
        {
            var match = _iDartServ.iDartMatchServ.Find(x => x.Id == matchId);
            match.EndDate = match.StartDate.AddMinutes(5);

            var winer = _iDartServ.iDartMatchUserServ.QueryMaker(y => y.Where(x => x.MatchId == matchId)).OrderByDescending(x => x.TotalScore).Skip(0).Take(1).ToList();
            string firstName = string.Empty;
            string lastName = string.Empty;
            if (winer.Count > 0)
            {
                match.UserId = winer[0].UserId;
                match.WinerScore = winer[0].TotalScore;
                firstName = winer[0].User.FirstName;
                lastName = winer[0].User.LastName;
            }
            await _iDartServ.iDartMatchServ.SaveChangesAsync();

            //_pushService.Clients.All.SendAsync("FinishGame", matchId);

            return Ok(new
            {
                title = "New Match",
                status = 200,
                data = new
                {
                    match.Id,
                    match.Title,
                    match.UserId,
                    match.WinerScore,
                    FirstName = firstName,
                    LastName = lastName
                }
            });
        }

        [HttpGet]
        public IActionResult GetTopWinners(int size)
        {
            var winers = _iDartServ.iDartMatchServ.QueryMaker(y => y.Where(x => x.EndDate != null && x.UserId != null)).Select(x => new
            {
                x.Id,
                x.Title,
                x.AppUser.FirstName,
                x.AppUser.LastName,
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
            var topClients = _iDartServ.iDartMatchUserServ.QueryMaker(y => y.Where(x => x.MatchId == matchId)).Select(x => new
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
                data = topClients
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
