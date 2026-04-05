using Backend.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shared.DTOs.Auth;
using Backend.Src.Models;
using Shared.Constants;
using Shared.DTOs;
using Shared.DTOs.Requests;
namespace Backend.Src.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoClient client, IOptions<MongoDBSettings> options)
        {
            _users = client.GetDatabase(options.Value.DatabaseName).GetCollection<User>("Users");
        }

        private async Task<User?> GetByUsernameAsync(string username)
        {
            return await _users.Find(u => u.Username == username)
                .FirstOrDefaultAsync();
        }

        private async Task<bool> UsernameExistsAsync(string username)
        {
            return await _users.Find(u => u.Username == username)
                .AnyAsync();
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

            if (!await UsernameExistsAsync(req.Username))
            {
                string passwordHash = CryptoService.Hash(req.Password);

                var user = new User()
                {
                    Username = req.Username,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.Now,
                    IsOnline = true
                };

                await _users.InsertOneAsync(user);

                var userDTO = new UserDTO()
                {
                    Username = user.Username,
                    CreatedAt = user.CreatedAt,
                    Id = user.Id,
                    IsOnline = user.IsOnline,
                    ProfileImageUrl = user.ProfileImageUrl
                };

                return new()
                {
                    Success = true,
                    User = userDTO,
                    Message = Messages.UserCreatedSuccess
                };
            }
            else
            {
                return new()
                {
                    Success = false,
                    User = null,
                    Message = Messages.UserNameAlreadyExists
                };
            }
        }

        public async Task<AuthResponse> Login(LoginRequest req)
        {
            var user = await GetByUsernameAsync(req.Username);

            if (user != null && CryptoService.VerifyHash(req.Password, user.PasswordHash))
            {
                user.IsOnline = true;

                return new()
                {
                    Success = true,
                    Message = Messages.LoginSuccess,
                    User = new()
                    {
                        CreatedAt = user.CreatedAt.ToLocalTime(),
                        Id = user.Id,
                        IsOnline = true,
                        ProfileImageUrl = user.ProfileImageUrl,
                        Username = user.Username
                    }
                };
            }

            return new()
            {
                Message = Messages.InvalidUsernameOrPassowrd,
                Success = false,
                User = null
            };
        }
    }
}