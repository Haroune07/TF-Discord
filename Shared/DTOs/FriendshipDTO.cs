using Shared.Enums;

namespace Shared.DTOs
{
    public class FriendshipDTO
    {
        public string Id { get; set; } = string.Empty;
        public UserDTO? Friend { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
