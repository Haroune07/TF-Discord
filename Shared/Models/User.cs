using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Shared.Models
{
    public class User
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public byte[]? ProfileImageData { get; set; }
        public bool IsOnline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.ToLocalTime();
    }
}
