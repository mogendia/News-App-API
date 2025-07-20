namespace NewsApp.Services
{
    public class LiveHub : Hub
    {
        public static int ConnectedUsers = 0;
        private static string AdminConnectionId = "";

        public override async Task OnConnectedAsync()
        {
            ConnectedUsers++;
            await Clients.All.SendAsync("UserCountChanged", ConnectedUsers);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            if (Context.ConnectionId == AdminConnectionId)
            {
                AdminConnectionId = "";
                await Clients.All.SendAsync("OnLiveEnded");
            }

            ConnectedUsers--;
            await Clients.All.SendAsync("UserCountChanged", ConnectedUsers);
            await base.OnDisconnectedAsync(ex);
        }

        public async Task StartLive(string title)
        {
            AdminConnectionId = Context.ConnectionId;
            await Clients.All.SendAsync("OnLiveStarted", title);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task GetViewerId()
        {
            var connectionId = Context.ConnectionId;
            await Clients.Caller.SendAsync("ReceiveViewerId", connectionId);
        }

        public async Task SendOfferToViewer(string viewerId, string sdp)
        {
            await Clients.Client(viewerId).SendAsync("ReceiveOffer", sdp);
        }

        public async Task SendAnswerToAdmin(string sdp)
        {
            var viewerId = Context.ConnectionId;
            if (!string.IsNullOrEmpty(AdminConnectionId))
            {
                await Clients.Client(AdminConnectionId).SendAsync("AnswerFromViewer", viewerId, sdp);
            }
        }

        public async Task SendCandidateToViewer(string viewerId, string candidate)
        {
            await Clients.Client(viewerId).SendAsync("CandidateFromAdmin", candidate);
        }

        public async Task SendCandidateToAdmin(string candidate)
        {
            var fromId = Context.ConnectionId;
            if (!string.IsNullOrEmpty(AdminConnectionId))
            {
                await Clients.Client(AdminConnectionId).SendAsync("CandidateFromViewer", fromId, candidate);
            }
        }

        public async Task SendCandidate(string targetConnectionId, string candidate)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveCandidate", Context.ConnectionId, candidate);
        }

        public async Task<bool> IsLive()
        {
            return !string.IsNullOrEmpty(AdminConnectionId);
        }
      

    }
}
