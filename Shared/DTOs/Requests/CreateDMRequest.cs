
namespace Shared.DTOs.Requests
{
    public class CreateDMRequest
    {
        public string SenderId { get; set; } = string.Empty;
        public string TargetUserId { get; set; } = string.Empty;
    }
}
