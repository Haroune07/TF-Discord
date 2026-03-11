using Backend.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shared.DTOs.Auth;
using Shared.Models;

namespace Backend.Src.Services
{
    public class UserService
    {

        private readonly IMongoCollection<User> _users;

        public UserService(IMongoClient client, IOptions<MongoDBSettings> options)
        {
            _users = client.GetDatabase(options.Value.DatabaseName).GetCollection<User>("Users");
        }

        public AuthResponse Register(RegisterRequest req)
        {

            var user = new User();

            user.Username = req.Username;
            user.PasswordHash = CryptoService.Hash(req.Password);

            _users.InsertOne(user);

            return new AuthResponse() { Message = "Utilisateur créé!", Success = true, User = user };

        }

    }
}
