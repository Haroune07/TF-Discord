namespace Shared.DTOs
{
    public class MessageDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
        public UserDTO Sender { get; set; } = new();
        public DateTime SentAt { get; set; }
    }
}
