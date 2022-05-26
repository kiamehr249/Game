using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NikGame.Web.PushService
{
    public class DartHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task AllowStartGame(int matchId)
        {
            await Clients.All.SendAsync("AllowStartGame", matchId);
        }

        public async Task UpdateScorePlayers(int matchId)
        {
            await Clients.All.SendAsync("UpdateScoreList", matchId);
        }
    }
}