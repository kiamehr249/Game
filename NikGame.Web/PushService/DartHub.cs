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

        public async Task UpdateMatchTops(int matchId)
        {
            await Clients.All.SendAsync("UpdateMatchTops", matchId);
        }

        public async Task FinishGame(int matchId)
        {
            await Clients.All.SendAsync("FinishGame", matchId);
        }
    }
}