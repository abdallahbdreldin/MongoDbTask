using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodayWebAPi.DAL.Data.Entities;

public class DeliveryMethod
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("shortName")]
    public string ShortName { get; set; }

    [BsonElement("deliveryTime")]
    public string DeliveryTime { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("price")]
    public decimal Price { get; set; }
}
