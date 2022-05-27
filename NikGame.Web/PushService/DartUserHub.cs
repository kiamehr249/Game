using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace NikGame.Web.PushService
{
    public class DartUserHub : Hub
    {
        private readonly IUserConnectionManager _userConnectionManager;
        public DartUserHub(IUserConnectionManager userConnectionManager)
        {
            _userConnectionManager = userConnectionManager;
        }

        public string GetConnectionId()
        {
            var httpContext = this.Context.GetHttpContext();
            var userId = httpContext.Request.Query["userId"];
            _userConnectionManager.KeepUserConnection(userId, Context.ConnectionId); 
            return Context.ConnectionId;
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            //get the connectionId
            var connectionId = Context.ConnectionId;
            _userConnectionManager.RemoveUserConnection(connectionId);
            var value = await Task.FromResult(0);
        }

        public async Task<bool> SendToUser(string userId, string name, string title, string message)
        {
            //await Clients.All.SendAsync("ReceiveData", userId, title, message);
            var connIds = _userConnectionManager.GetUserConnections(userId);
            if (connIds != null && connIds.Count > 0)
            {
                foreach (var connId in connIds)
                {
                    await Clients.Client(connId).SendAsync("ReceiveData", userId, name, title, message);
                }
                return true;
            }

            return false;
        }

    }
}
