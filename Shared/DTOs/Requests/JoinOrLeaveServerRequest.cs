namespace Shared.DTOs.Requests
{
    public class JoinOrLeaveServerRequest
    {
        public string ServerId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
