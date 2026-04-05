using Shared.Enums;
using System;

namespace Shared.DTOs
{
    public class ServerMemberDTO
    {
        public string Id { get; set; } = string.Empty;
        public string ServerId { get; set; } = string.Empty;
        public UserDTO User { get; set; } = new();
        public MemberRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
