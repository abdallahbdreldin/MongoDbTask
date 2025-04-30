using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodayWebAPi.DAL.Data.Models
{
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string StripePaymentIntentId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "usd";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
