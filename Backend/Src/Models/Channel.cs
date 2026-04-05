using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Enums;

namespace Backend.Src.Models
{
    public class Channel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public ChannelType Type { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ServerId { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public DateTime CreatedAt { get; set; }
        public List<string>? Participants { get; set; }
    }
}
