using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodayWebAPi.DAL.Data.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("pictureUrl")]
        public string? PictureUrl { get; set; }

        [BsonElement("inStock")]
        public int InStock { get; set; }

        [BsonRepresentation(BsonType.ObjectId)] 
        public string? BrandId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? TypeId { get; set; }

        //[BsonElement("brand")]
        //public ProductBrand Brand { get; set; }

        //[BsonElement("type")]
        //public ProductType Type { get; set; }
    }

}
