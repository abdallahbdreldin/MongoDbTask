using AspNetCore.Identity.Mongo.Model;

namespace TodayWebAPi.DAL.Data.Identity
{
    public class Role : MongoRole<string>
    {
        public Role()
        {
           
        }

        public Role(string RoleName) : base(RoleName)
        {
           
        }
    }
}
