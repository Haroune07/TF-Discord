using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Enums;

namespace Backend.Src.Models
{
    public class Channel : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public ChannelType Type { get; set; }

        public string ServerId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public List<string>? Participants { get; set; }
    }
}
