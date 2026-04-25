namespace Shared.DTOs.Requests
{
    public class KickMemberRequest
    {
        public string ServerId { get; set; } = string.Empty;
        public string TargetUserId { get; set; } = string.Empty;
        public string RequesterId { get; set; } = string.Empty;
    }
}