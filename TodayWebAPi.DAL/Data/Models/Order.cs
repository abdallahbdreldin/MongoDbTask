using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodayWebAPi.DAL.Data.Models
{
    public class Order
    {
        public Order()
        {
        }

        public Order(IReadOnlyList<OrderItem> orderItems, string buyerEmail, string deliveryMethodId, decimal subtotal)
        {
            OrderItems = orderItems;
            BuyerEmail = buyerEmail;
            DeliveryMethodId = deliveryMethodId;
            SubTotal = subtotal;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("buyerEmail")]
        public string BuyerEmail { get; set; }

        [BsonElement("orderDate")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [BsonElement("deliveryMethod")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? DeliveryMethodId { get; set; }

        [BsonElement("orderItems")]
        public IReadOnlyList<OrderItem> OrderItems { get; set; }

        [BsonElement("subtotal")]
        public decimal SubTotal { get; set; }

        [BsonElement("total")]
        public decimal Total {  get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public string Status { get; set; } = OrderStatus.Pending;

        [BsonElement("paymentIntentId")]
        public string? PaymentIntentId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PaymentId { get; set; }

    }

    public class OrderItem
    {
        public OrderItem()
        {
        }

        public OrderItem(string productId, decimal price, int quantity)
        {
            PrdouctId = productId;
            Price = price;
            Quantity = quantity;
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PrdouctId { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }
    }
}
