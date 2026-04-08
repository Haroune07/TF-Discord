using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Enums;

namespace Backend.Src.Models
{
    public class ServerMember : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ServerId { get; set; } = string.Empty;
        public MemberRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
