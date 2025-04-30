using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodayWebAPi.DAL.Data.Models
{
    public class CustomerBasket
    {
        public CustomerBasket()
        {
        }

        public CustomerBasket(string id)
        {
            Id = id;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("items")]
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        [BsonElement("deliveryMethodId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? DeliveryMethodId { get; set; }

        [BsonElement("clientSecret")]
        public string? ClientSecret { get; set; }

        [BsonElement("paymentIntentId")]
        public string? PaymentIntentId { get; set; }
    }
}