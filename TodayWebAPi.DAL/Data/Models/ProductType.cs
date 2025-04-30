using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodayWebAPi.DAL.Data.Models
{
    public class ProductType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }
    }

}
