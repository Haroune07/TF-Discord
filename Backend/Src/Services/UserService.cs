using Backend.Src.Mappers;
using Backend.Src.Models;
using Backend.Src.Repository;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Shared.Constants;
using Shared.DTOs;
using Shared.DTOs.Auth;
using Shared.DTOs.Requests;
namespace Backend.Src.Services
{
    public class UserService
    {
        private readonly IRepository<User> _users;

        public UserService(IRepository<User> userRepo)
        {
            _users = userRepo;
        }

        public async Task<List<UserDTO>> GetAllUsersExceptAsync(string userId)
        {
            var users = await _users.FindAsync(u => u.Id != userId);
            return users.Select(u => u.ToDTO()!).ToList();
        }

        private async Task<User?> GetByUsernameAsync(string username)
        {
            return (await _users.FindAsync(u => u.Username == username)).FirstOrDefault();
        }

        private async Task<bool> UsernameExistsAsync(string username)
        {
            return (await _users.FindAsync(u => u.Username == username)).Any();
        }

        public async Task<AuthResponse> Register(RegisterRequest req)
        {

            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            {
                return new()
                {
                    Success = false,
                    Message = Messages.InvalidUsernameOrPassowrd
                };
            }

            if (req.Username.Length > Auth.MaxUsernameLength)
            {
                return new()
                {
                    Success = false,
                    Message = Messages.InvalidUsernameLength
                };
            }

            if (req.Password.Length < Auth.MinPasswordLength)
            {
                return new()
                {
                    Success = false,
                    Message = Messages.InvalidPasswordLength
                };
            }

            if (!await UsernameExistsAsync(req.Username))
            {
                string passwordHash = CryptoService.Hash(req.Password);
                var user = new User() { Username = req.Username, PasswordHash = passwordHash, CreatedAt = DateTime.UtcNow };
                await _users.InsertAsync(user);
                var userDTO = user.ToDTO();
                return new() { Success = true, User = userDTO, Message = Messages.UserCreatedSuccess };
            }

            else
            {
                return new() { Success = false, User = null, Message = Messages.UserNameAlreadyExists };
            }

        }

        public async Task<AuthResponse> Login(LoginRequest req)
        {
            if (await UsernameExistsAsync(req.Username))
            {
                var user = await GetByUsernameAsync(req.Username);

                if (CryptoService.VerifyHash(req.Password, user.PasswordHash))
                {
                    
                    await _users.UpdateAsync(user.Id, user);
                    return new()
                    {
                        Success = true,
                        Message = Messages.LoginSuccess,
                        User = user.ToDTO()
                    };
                }
            }

            return new()
            {
                Message = Messages.InvalidUsernameOrPassowrd,
                Success = false,
                User = null
            };
        }

        public async Task<bool> UpdatePfpAsync(string userId, string newPfpUrl)
        {
            var user = await _users.GetByIdAsync(userId);

            if (user == null) return false;

            user.ProfileImageUrl = newPfpUrl;

            await _users.UpdateAsync(userId, user);

            return true;
        }

        public async Task<bool> UpdateStatusAsync(string userId, string newStatus)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user == null) return false;

            // Convertit "True" (envoyé par le XAML/JSON) en booléen pour le modèle User
            if (bool.TryParse(newStatus, out bool isOnline))
            {
                user.IsOnline = isOnline;
            }
            else
            {
                // Optionnel : gestion si le string est "online"/"offline"
                user.IsOnline = newStatus.Equals("online", StringComparison.OrdinalIgnoreCase);
            }

            await _users.UpdateAsync(userId, user);
            return true;
        }

    }
}
