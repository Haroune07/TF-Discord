using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Enums;

namespace Backend.Src.Models
{
    public class Friendship : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string RequesterId { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ReceiverId { get; set; } = string.Empty;

        public FriendshipStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
