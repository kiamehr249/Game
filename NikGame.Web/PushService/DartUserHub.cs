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

    }
}
