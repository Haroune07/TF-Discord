namespace Shared.DTOs.Requests
{
    public class EditMessageRequest
    {
        public string RequesterId { get; set; } = string.Empty;
        public string NewContent { get; set; } = string.Empty;
    }
}
