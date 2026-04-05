using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Src.Models
{
    public class Server
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
