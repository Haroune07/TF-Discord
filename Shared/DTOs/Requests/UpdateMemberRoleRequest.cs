using Shared.Enums;

namespace Shared.DTOs.Requests
{
    public class UpdateMemberRoleRequest
    {
        public string RequesterId { get; set; } = string.Empty;
        public MemberRole NewRole { get; set; }
    }
}