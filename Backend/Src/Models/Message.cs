using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Src.Models
{
    public class Message : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string SenderId { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ChannelId { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}
