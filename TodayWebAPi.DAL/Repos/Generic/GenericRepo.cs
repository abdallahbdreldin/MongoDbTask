using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using TodayWebAPi.DAL.Data.Context;
namespace TodayWebAPi.DAL.Repos.Generic
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        private readonly StoreContext _context;

        public GenericRepo(StoreContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var collection = GetCollection();
            var objectId = new ObjectId(id);
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            var collection = GetCollection();
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task AddAsync(T item)
        {
            var collection = GetCollection();
            await collection.InsertOneAsync(item);
        }

        public async Task DeleteAsync(string id)
        {
            var collection = GetCollection();
            var objectId = new ObjectId(id);
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            await collection.DeleteOneAsync(filter);
        }

        public async Task UpdateAsync(T item)
        {
            var collection = GetCollection();
            var filter = Builders<T>.Filter.Eq("_id", item.GetType().GetProperty("Id")?.GetValue(item).ToString());
            await collection.ReplaceOneAsync(filter , item);
        }

        public async Task<T> FindOneAsync(Expression<Func<T, bool>> filter)
        {
            var collection = GetCollection();
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> FindAllAsync(Expression<Func<T, bool>> filter)
        {
            var collection = GetCollection();
            return await collection.Find(filter).ToListAsync();
        }

        public IMongoCollection<T> GetCollection()
        {
            return _context.GetCollection<T>();
        }
    }
}
