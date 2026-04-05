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

        public const string CreateServerChannelRoute = "/api/channel/server";
        public const string CreateDMChannelRoute = "/api/channel/dm";
        public const string GetServerChannelsRoute = "/api/channel/server";

        public const string GetChannelMessagesRoute = "/api/message";
        public const string SendMessageRoute = "/api/message/send";

    }
}
