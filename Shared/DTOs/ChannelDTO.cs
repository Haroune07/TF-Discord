using Shared.Enums;
namespace Shared.DTOs
{
    public class ChannelDTO
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public ChannelType Type { get; set; }
        public string? ServerId { get; set; }
        public List<UserDTO>? Participants { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
