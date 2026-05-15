namespace Shared.Constants
{
    public class Routes
    {
        public const string LoginRoute = "/api/auth/login";
        public const string RegisterRoute = "/api/auth/register";

        public const string CreateServerRoute = "/api/server/create";
        public const string GetMyServersRoute = "/api/server/mine";
        public const string JoinServerRoute = "/api/server/join";
        public const string LeaveServerRoute = "/api/server/leave";
        public const string GetAllServersRoute = "/api/server/all";

        public const string CreateServerChannelRoute = "/api/channel/server";
        public const string DeleteChannelRoute = "/api/channel";
        public const string CreateDMChannelRoute = "/api/channel/dm";
        public const string GetServerChannelsRoute = "/api/channel/server";

        public const string GetChannelMessagesRoute = "/api/message";
        public const string SendMessageRoute = "/api/message/send";
        public const string EditMessageRoute = "/api/message";
        public const string DeleteMessageRoute = "/api/message";

        public const string GetAllUsersRoute = "/api/user/all";
        public const string SearchUser = "/api/user/search";

        public const string UpdateStatus = "/api/user/{0}/update-status";
        public const string UpdatePfp = "/api/user/{0}/update-pfp";

        public const string SendFriendRequestRoute = "/api/friendship/request";
        public const string GetFriendsRoute = "/api/friendship/friends";
        public const string GetPendingFriendsRoute = "/api/friendship/pending";
        public const string UpdateFriendshipStatusRoute = "/api/friendship";
    }
}
