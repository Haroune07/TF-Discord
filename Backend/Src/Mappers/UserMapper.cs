using Backend.Src.Models;
using Shared.DTOs;

namespace Backend.Src.Mappers
{
    public static class UserMapper
    {
        public static UserDTO? ToDTO(this User? user)
        {
            if (user == null) return null;

            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                IsOnline = user.IsOnline,
                CreatedAt = user.CreatedAt,
                ProfileImageUrl = user.ProfileImageUrl
            };
        }
    }
}
