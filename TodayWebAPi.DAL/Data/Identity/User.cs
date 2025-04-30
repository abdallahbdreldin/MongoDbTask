using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodayWebAPi.DAL.Data.Identity
{
    public class User : MongoUser<string>
    {
        [BsonElement("DisplayName")]
        public string DisplayName { get; set; }
    }
}
