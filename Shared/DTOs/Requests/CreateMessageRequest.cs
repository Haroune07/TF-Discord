
namespace Shared.DTOs.Requests
{
    public class CreateMessageRequest
    {
        public string Content { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
    }
}
