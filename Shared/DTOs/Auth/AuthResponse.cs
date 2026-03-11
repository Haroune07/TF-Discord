using Shared.Models;

namespace Shared.DTOs.Auth
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}
